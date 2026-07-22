window.appDownload = {
    base64ToBlobUrl: function (contentType, base64) {
        const byteChars = atob(base64);
        const byteNumbers = new Uint8Array(byteChars.length);
        for (let i = 0; i < byteChars.length; i++) {
            byteNumbers[i] = byteChars.charCodeAt(i);
        }
        const blob = new Blob([byteNumbers], { type: contentType || 'application/octet-stream' });
        return URL.createObjectURL(blob);
    },

    // Faz download somente quando o usuário aciona o botão específico de download.
    fromBytes: function (fileName, contentType, base64) {
        const url = this.base64ToBlobUrl(contentType, base64);
        const link = document.createElement('a');
        link.href = url;
        link.download = fileName || 'arquivo';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.setTimeout(() => URL.revokeObjectURL(url), 60000);
    },

    // Apenas visualiza PDF ou imagem. Nunca inicia download automaticamente.
    openFile: function (fileName, contentType, base64) {
        const type = (contentType || '').toLowerCase();
        const canPreview = type === 'application/pdf' || type.startsWith('image/');
        if (!canPreview) {
            window.alert('Este formato não possui visualização no navegador. Use o botão de download.');
            return false;
        }

        const url = this.base64ToBlobUrl(contentType, base64);
        const opened = window.open('', '_blank');
        if (!opened) {
            URL.revokeObjectURL(url);
            window.alert('O navegador bloqueou a abertura do arquivo. Permita pop-ups para este site e tente novamente.');
            return false;
        }

        opened.opener = null;
        const safeTitle = (fileName || 'Arquivo').replace(/[<>&"']/g, '');
        opened.document.write(`<!doctype html><html><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>${safeTitle}</title><style>html,body{margin:0;width:100%;height:100%;background:#111}iframe,img{display:block;width:100%;height:100%;border:0;object-fit:contain}</style></head><body>${type.startsWith('image/') ? `<img src="${url}" alt="${safeTitle}">` : `<iframe src="${url}" title="${safeTitle}"></iframe>`}</body></html>`);
        opened.document.close();
        window.setTimeout(() => URL.revokeObjectURL(url), 10 * 60 * 1000);
        return true;
    },

    openPdf: function (base64) {
        return this.openFile('arquivo.pdf', 'application/pdf', base64);
    }
};
