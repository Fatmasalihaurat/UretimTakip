// Site-wide SweetAlert2 & Notification Utilities

// SweetAlert2 Toast helper
window.showToast = function (icon, title, message, timer = 4000) {
    if (typeof Swal === 'undefined') return;
    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: timer,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.onmouseenter = Swal.stopTimer;
            toast.onmouseleave = Swal.resumeTimer;
        }
    });

    return Toast.fire({
        icon: icon || 'info',
        title: title || '',
        text: message || undefined
    });
};

// SweetAlert2 Modal Alert helper
window.showAlert = function (icon, title, text) {
    if (typeof Swal === 'undefined') {
        window._nativeAlert ? window._nativeAlert(text || title) : alert(text || title);
        return Promise.resolve();
    }
    return Swal.fire({
        icon: icon || 'info',
        title: title || 'Bilgilendirme',
        text: text || '',
        confirmButtonText: 'Tamam',
        confirmButtonColor: '#0d6efd'
    });
};

// SweetAlert2 Confirmation helper
window.showConfirm = function (options) {
    if (typeof Swal === 'undefined') {
        const ok = confirm(options.text || options.title || 'İşlemi onaylıyor musunuz?');
        if (ok && typeof options.onConfirm === 'function') options.onConfirm();
        return;
    }
    return Swal.fire({
        title: options.title || 'Emin misiniz?',
        text: options.text || 'Bu işlemi gerçekleştirmek istediğinize emin misiniz?',
        icon: options.icon || 'warning',
        showCancelButton: true,
        confirmButtonColor: options.confirmButtonColor || '#dc3545',
        cancelButtonColor: options.cancelButtonColor || '#6c757d',
        confirmButtonText: options.confirmButtonText || 'Evet, Onaylıyorum',
        cancelButtonText: options.cancelButtonText || 'Vazgeç',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            if (typeof options.onConfirm === 'function') {
                options.onConfirm();
            }
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            if (typeof options.onCancel === 'function') {
                options.onCancel();
            }
        }
    });
};

// Override window.alert so that any standard browser alert call renders as a modern SweetAlert2 modal
if (!window._nativeAlert) {
    window._nativeAlert = window.alert;
    window.alert = function (message) {
        if (typeof Swal !== 'undefined') {
            const msgStr = (message !== undefined && message !== null) ? message.toString() : '';
            const isError = /hata|error|uyarı|dikkat|silinemez|eksiksiz|geçersiz|bulunamadı/i.test(msgStr);
            const isSuccess = /başarı|oluşturuldu|güncellendi|silindi|eklendi|aktifleştirildi|kaydedildi/i.test(msgStr);
            const icon = isError ? 'warning' : (isSuccess ? 'success' : 'info');
            const title = isError ? 'Uyarı' : (isSuccess ? 'Başarılı' : 'Bilgi');

            Swal.fire({
                icon: icon,
                title: title,
                text: msgStr,
                confirmButtonText: 'Tamam',
                confirmButtonColor: '#0d6efd'
            });
        } else {
            window._nativeAlert(message);
        }
    };
}
