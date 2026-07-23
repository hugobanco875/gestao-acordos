window.agreementUpload = (() => {
    const maxSize = 10 * 1024 * 1024;
    const allowedExtensions = ['.pdf', '.docx', '.jpg', '.jpeg', '.png'];

    function extensionPermitida(fileName) {
        const nome = (fileName || '').toLowerCase();
        return allowedExtensions.some(ext => nome.endsWith(ext));
    }

    function uploadOne(file, endpoint, itemId, dotNetRef) {
        return new Promise((resolve) => {
            // Falha rápido, sem gastar tempo de rede, quando o arquivo já não passaria
            // na validação do servidor (tamanho ou formato).
            if (file.size > maxSize) {
                dotNetRef.invokeMethodAsync('FalharUpload', itemId, 'O arquivo ultrapassa o limite de 10 MB.');
                resolve(false);
                return;
            }
            if (!extensionPermitida(file.name)) {
                dotNetRef.invokeMethodAsync('FalharUpload', itemId, 'Formato não permitido. Use PDF, DOCX, JPG, JPEG ou PNG.');
                resolve(false);
                return;
            }

            const xhr = new XMLHttpRequest();
            const form = new FormData();
            form.append('file', file, file.name);

            xhr.open('POST', endpoint, true);
            xhr.withCredentials = true;

            let ultimoPercentualNotificado = -1;
            xhr.upload.onprogress = (event) => {
                if (!event.lengthComputable) return;
                const percent = Math.min(99, Math.round((event.loaded / event.total) * 100));
                // Notifica o componente Blazor apenas a cada 5% para reduzir o número
                // de round-trips ao servidor e manter o upload rápido e responsivo.
                if (percent - ultimoPercentualNotificado >= 5 || percent === 99) {
                    ultimoPercentualNotificado = percent;
                    dotNetRef.invokeMethodAsync('AtualizarProgressoUpload', itemId, percent);
                }
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

        // Mais uploads simultâneos aceleram o envio de lotes com vários arquivos,
        // respeitando o limite de conexões paralelas por origem dos navegadores.
        const workers = [];
        const count = Math.max(1, Math.min(maxParallel || 4, queue.length));
        for (let i = 0; i < count; i++) workers.push(worker());
        await Promise.all(workers);
        input.value = '';
        await dotNetRef.invokeMethodAsync('FinalizarLoteUpload');
    }

    return { importFiles };
})();
