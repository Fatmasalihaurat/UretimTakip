// =========================================================================
// Site-wide SweetAlert2 & Notification Helper Utilities
// =========================================================================

// Modern SweetAlert2 Toast Helper
// Arguments flexible:
//   showToast('success', 'İşlem Başarılı')
//   showToast('success', 'Başlık', 'Detay mesajı', 3500)
//   showToast('Mesaj metni')
window.showToast = function (arg1, arg2, arg3, arg4) {
    if (typeof Swal === 'undefined') {
        console.warn('SweetAlert2 (Swal) yüklenemediği için toast gösterilemiyor.');
        return Promise.resolve();
    }

    const validIcons = ['success', 'error', 'warning', 'info', 'question'];
    let icon = 'info';
    let title = '';
    let message = '';
    let timer = 3500;

    if (arguments.length === 1) {
        // Tek parametre girilmişse doğrudan başlık/mesaj kabul et
        title = arg1 ? String(arg1) : '';
    } else if (arguments.length === 2) {
        if (validIcons.includes(arg1)) {
            icon = arg1;
            title = arg2 ? String(arg2) : '';
        } else {
            title = arg1 ? String(arg1) : '';
            message = arg2 ? String(arg2) : '';
        }
    } else {
        icon = validIcons.includes(arg1) ? arg1 : 'info';
        title = arg2 ? String(arg2) : '';
        message = arg3 ? String(arg3) : '';
        if (typeof arg4 === 'number' && arg4 > 0) {
            timer = arg4;
        }
    }

    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        showCloseButton: true,
        timer: timer,
        timerProgressBar: true,
        didOpen: (toast) => {
            toast.onmouseenter = Swal.stopTimer;
            toast.onmouseleave = Swal.resumeTimer;
        }
    });

    return Toast.fire({
        icon: icon,
        title: title || undefined,
        text: message || undefined
    });
};

// Modern SweetAlert2 Modal Alert Helper
window.showAlert = function (icon, title, text) {
    if (typeof Swal === 'undefined') {
        const fullMsg = (title ? title + ': ' : '') + (text || '');
        if (window._nativeAlert) {
            window._nativeAlert(fullMsg);
        } else {
            alert(fullMsg);
        }
        return Promise.resolve();
    }

    const validIcons = ['success', 'error', 'warning', 'info', 'question'];
    const iconType = validIcons.includes(icon) ? icon : 'info';

    return Swal.fire({
        icon: iconType,
        title: title || (iconType === 'error' ? 'Hata' : (iconType === 'warning' ? 'Uyarı' : 'Bilgi')),
        text: text || '',
        confirmButtonText: 'Tamam',
        confirmButtonColor: '#0d6efd'
    });
};

// Modern SweetAlert2 Confirmation Dialog Helper
window.showConfirm = function (options) {
    options = options || {};

    if (typeof Swal === 'undefined') {
        const ok = confirm(options.text || options.title || 'Bu işlemi gerçekleştirmek istediğinize emin misiniz?');
        if (ok && typeof options.onConfirm === 'function') {
            options.onConfirm();
        } else if (!ok && typeof options.onCancel === 'function') {
            options.onCancel();
        }
        return Promise.resolve({ isConfirmed: ok });
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
        reverseButtons: true,
        focusCancel: true
    }).then((result) => {
        if (result.isConfirmed) {
            if (typeof options.onConfirm === 'function') {
                options.onConfirm();
            }
        } else if (result.dismiss === Swal.DismissReason.cancel || result.isDismissed) {
            if (typeof options.onCancel === 'function') {
                options.onCancel();
            }
        }
        return result;
    });
};

// Standart browser alert çağrılarını şık SweetAlert2 modalına dönüştür
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
