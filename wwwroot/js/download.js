window.appDownload = {
    // Baixa um arquivo a partir de bytes (base64) vindos do C#.
    fromBytes: function (fileName, contentType, base64) {
        const link = document.createElement('a');
        link.href = 'data:' + contentType + ';base64,' + base64;
        link.download = fileName;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    },
    // Abre um PDF em nova aba.
    openPdf: function (base64) {
        const byteChars = atob(base64);
        const byteNumbers = new Array(byteChars.length);
        for (let i = 0; i < byteChars.length; i++) {
            byteNumbers[i] = byteChars.charCodeAt(i);
        }
        const blob = new Blob([new Uint8Array(byteNumbers)], { type: 'application/pdf' });
        const url = URL.createObjectURL(blob);
        window.open(url, '_blank');
    }
};
