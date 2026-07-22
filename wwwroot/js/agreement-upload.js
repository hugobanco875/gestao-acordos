window.agreementUpload = (() => {
    function uploadOne(file, endpoint, itemId, dotNetRef) {
        return new Promise((resolve) => {
            const xhr = new XMLHttpRequest();
            const form = new FormData();
            form.append('file', file, file.name);

            xhr.open('POST', endpoint, true);
            xhr.withCredentials = true;

            xhr.upload.onprogress = (event) => {
                if (!event.lengthComputable) return;
                const percent = Math.min(99, Math.round((event.loaded / event.total) * 100));
                dotNetRef.invokeMethodAsync('AtualizarProgressoUpload', itemId, percent);
            };

            xhr.onload = async () => {
                if (xhr.status >= 200 && xhr.status < 300) {
                    try {
                        const result = JSON.parse(xhr.responseText);
                        await dotNetRef.invokeMethodAsync(
                            'ConcluirUpload',
                            itemId,
                            result.token,
                            result.name,
                            result.contentType,
                            result.size);
                        resolve(true);
                    } catch (error) {
                        await dotNetRef.invokeMethodAsync('FalharUpload', itemId, 'Resposta inválida do servidor.');
                        resolve(false);
                    }
                } else {
                    let message = 'Falha ao importar o arquivo.';
                    try {
                        const body = JSON.parse(xhr.responseText);
                        message = body.error || body.detail || message;
                    } catch { }
                    await dotNetRef.invokeMethodAsync('FalharUpload', itemId, message);
                    resolve(false);
                }
            };

            xhr.onerror = async () => {
                await dotNetRef.invokeMethodAsync('FalharUpload', itemId, 'Falha de conexão durante o upload.');
                resolve(false);
            };

            xhr.send(form);
        });
    }

    async function importFiles(inputId, endpoint, items, dotNetRef, maxParallel) {
        const input = document.getElementById(inputId);
        if (!input || !input.files || input.files.length === 0) {
            await dotNetRef.invokeMethodAsync('FinalizarLoteUpload');
            return;
        }

        const files = Array.from(input.files);
        const queue = files.map((file, index) => ({ file, item: items[index] }));
        let cursor = 0;

        async function worker() {
            while (true) {
                const index = cursor++;
                if (index >= queue.length) return;
                const current = queue[index];
                await uploadOne(current.file, endpoint, current.item.id, dotNetRef);
            }
        }

        const workers = [];
        const count = Math.max(1, Math.min(maxParallel || 3, queue.length));
        for (let i = 0; i < count; i++) workers.push(worker());
        await Promise.all(workers);
        input.value = '';
        await dotNetRef.invokeMethodAsync('FinalizarLoteUpload');
    }

    return { importFiles };
})();
