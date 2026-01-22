// GrowingTales - Main JavaScript

// Text-to-Speech functionality
const TTS = {
    synth: window.speechSynthesis,
    utterance: null,
    isSupported: 'speechSynthesis' in window,

    speak: function(text, rate = 1.0) {
        if (!this.isSupported) {
            alert('Trinh duyet cua ban khong ho tro doc truyen bang giong noi.');
            return;
        }
        
        this.stop();
        this.utterance = new SpeechSynthesisUtterance(text);
        this.utterance.lang = 'vi-VN';
        this.utterance.rate = rate;
        
        // Try to find Vietnamese voice
        const voices = this.synth.getVoices();
        const viVoice = voices.find(v => v.lang.includes('vi'));
        if (viVoice) {
            this.utterance.voice = viVoice;
        }
        
        this.synth.speak(this.utterance);
    },

    pause: function() {
        if (this.synth.speaking) {
            this.synth.pause();
        }
    },

    resume: function() {
        if (this.synth.paused) {
            this.synth.resume();
        }
    },

    stop: function() {
        this.synth.cancel();
    },

    isSpeaking: function() {
        return this.synth.speaking;
    },

    isPaused: function() {
        return this.synth.paused;
    }
};

// Audio Recording functionality
const AudioRecorder = {
    mediaRecorder: null,
    audioChunks: [],
    isRecording: false,

    init: async function() {
        try {
            const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
            this.mediaRecorder = new MediaRecorder(stream);
            
            this.mediaRecorder.ondataavailable = (event) => {
                this.audioChunks.push(event.data);
            };
            
            return true;
        } catch (error) {
            console.error('Khong the truy cap microphone:', error);
            return false;
        }
    },

    start: function() {
        if (this.mediaRecorder && this.mediaRecorder.state === 'inactive') {
            this.audioChunks = [];
            this.mediaRecorder.start();
            this.isRecording = true;
        }
    },

    stop: function() {
        return new Promise((resolve) => {
            if (this.mediaRecorder && this.mediaRecorder.state === 'recording') {
                this.mediaRecorder.onstop = () => {
                    const audioBlob = new Blob(this.audioChunks, { type: 'audio/webm' });
                    this.isRecording = false;
                    resolve(audioBlob);
                };
                this.mediaRecorder.stop();
            } else {
                resolve(null);
            }
        });
    },

    getBase64: function(blob) {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.onloadend = () => {
                const base64 = reader.result.split(',')[1];
                resolve(base64);
            };
            reader.onerror = reject;
            reader.readAsDataURL(blob);
        });
    }
};

// Utility functions
function showLoading(message = 'Dang xu ly...') {
    const overlay = document.createElement('div');
    overlay.id = 'loading-overlay';
    overlay.className = 'spinner-overlay';
    overlay.innerHTML = `
        <div class="text-center">
            <div class="spinner-border text-primary" style="width: 3rem; height: 3rem;" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
            <p class="mt-3 fw-bold">${message}</p>
        </div>
    `;
    document.body.appendChild(overlay);
}

function hideLoading() {
    const overlay = document.getElementById('loading-overlay');
    if (overlay) {
        overlay.remove();
    }
}

function showToast(message, type = 'success') {
    const toastContainer = document.getElementById('toast-container') || createToastContainer();
    
    const toast = document.createElement('div');
    toast.className = `toast align-items-center text-white bg-${type === 'error' ? 'danger' : type} border-0`;
    toast.setAttribute('role', 'alert');
    toast.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">${message}</div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>
    `;
    
    toastContainer.appendChild(toast);
    const bsToast = new bootstrap.Toast(toast);
    bsToast.show();
    
    toast.addEventListener('hidden.bs.toast', () => toast.remove());
}

function createToastContainer() {
    const container = document.createElement('div');
    container.id = 'toast-container';
    container.className = 'toast-container position-fixed bottom-0 end-0 p-3';
    document.body.appendChild(container);
    return container;
}

// Form validation helpers
function validateForm(formId) {
    const form = document.getElementById(formId);
    if (!form) return true;
    
    if (!form.checkValidity()) {
        form.classList.add('was-validated');
        return false;
    }
    return true;
}

// Image preview
function previewImage(input, previewId) {
    if (input.files && input.files[0]) {
        const reader = new FileReader();
        reader.onload = function(e) {
            document.getElementById(previewId).src = e.target.result;
        };
        reader.readAsDataURL(input.files[0]);
    }
}

// Copy to clipboard
async function copyToClipboard(text) {
    try {
        await navigator.clipboard.writeText(text);
        showToast('Da sao chep!', 'success');
    } catch (err) {
        console.error('Khong the sao chep:', err);
        showToast('Khong the sao chep', 'error');
    }
}

// Confirm delete
function confirmDelete(message = 'Ban co chac chan muon xoa?') {
    return confirm(message);
}

// Initialize on DOM ready
document.addEventListener('DOMContentLoaded', function() {
    // Initialize tooltips
    const tooltips = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    tooltips.forEach(el => new bootstrap.Tooltip(el));

    // Initialize popovers
    const popovers = document.querySelectorAll('[data-bs-toggle="popover"]');
    popovers.forEach(el => new bootstrap.Popover(el));

    // Auto-hide alerts
    const alerts = document.querySelectorAll('.alert-dismissible');
    alerts.forEach(alert => {
        setTimeout(() => {
            const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
            bsAlert.close();
        }, 5000);
    });
});
