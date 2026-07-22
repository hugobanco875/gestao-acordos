(() => {
    const onlyDigits = (value, max) => (value || '').replace(/\D/g, '').slice(0, max);

    function maskCpfCnpj(value) {
        const digits = onlyDigits(value, 14);
        if (digits.length <= 11) {
            return digits
                .replace(/^(\d{3})(\d)/, '$1.$2')
                .replace(/^(\d{3})\.(\d{3})(\d)/, '$1.$2.$3')
                .replace(/\.(\d{3})(\d)/, '.$1-$2');
        }
        return digits
            .replace(/^(\d{2})(\d)/, '$1.$2')
            .replace(/^(\d{2})\.(\d{3})(\d)/, '$1.$2.$3')
            .replace(/\.(\d{3})(\d)/, '.$1/$2')
            .replace(/(\d{4})(\d)/, '$1-$2');
    }

    function maskCep(value) {
        return onlyDigits(value, 8).replace(/^(\d{5})(\d)/, '$1-$2');
    }

    function maskProcesso(value) {
        const d = onlyDigits(value, 20);
        const groups = [7, 2, 4, 1, 2, 4];
        const separators = ['-', '.', '.', '.', '.'];
        let result = '';
        let index = 0;
        for (let i = 0; i < groups.length && index < d.length; i++) {
            const chunk = d.slice(index, index + groups[i]);
            result += chunk;
            index += chunk.length;
            if (chunk.length === groups[i] && index < d.length && i < separators.length) {
                result += separators[i];
            }
        }
        return result;
    }

    function applyMask(input) {
        const type = input?.dataset?.mask;
        if (!type) return;
        const cursorAtEnd = input.selectionStart === input.value.length;
        const before = input.value;
        if (type === 'cpf-cnpj') input.value = maskCpfCnpj(before);
        if (type === 'cep') input.value = maskCep(before);
        if (type === 'processo') input.value = maskProcesso(before);
        if (cursorAtEnd && document.activeElement === input) {
            input.setSelectionRange(input.value.length, input.value.length);
        }
    }

    document.addEventListener('input', event => {
        if (event.target instanceof HTMLInputElement && event.target.matches('[data-mask]')) {
            applyMask(event.target);
        }
    }, true);

    document.addEventListener('focusin', event => {
        if (event.target instanceof HTMLInputElement && event.target.matches('[data-mask]')) {
            applyMask(event.target);
        }
    });

    const formatAll = root => root.querySelectorAll?.('input[data-mask]').forEach(applyMask);
    document.addEventListener('DOMContentLoaded', () => formatAll(document));
    new MutationObserver(records => {
        for (const record of records) {
            for (const node of record.addedNodes) {
                if (!(node instanceof Element)) continue;
                if (node.matches?.('input[data-mask]')) applyMask(node);
                formatAll(node);
            }
        }
    }).observe(document.documentElement, { childList: true, subtree: true });
})();
