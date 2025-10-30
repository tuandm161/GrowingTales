import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { StoryService } from '../../services/story.service';
import { StoryRequest, AudioStoryRequest } from '../../models/story.model';

@Component({
  selector: 'app-create-story',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './create-story.component.html',
  styleUrl: './create-story.component.css'
})
export class CreateStoryComponent {
  // Form fields
  inputMode = signal<'text' | 'audio'>('text');
  inputText = signal<string>('');
  childName = signal<string>('');
  childAge = signal<number>(5);
  theme = signal<string>('');
  additionalContext = signal<string>('');
  pageCount = signal<number>(5);

  // Audio recording
  isRecording = signal<boolean>(false);
  audioBlob = signal<Blob | null>(null);
  audioUrl = signal<string>('');
  mediaRecorder: MediaRecorder | null = null;

  // UI state
  loading = signal<boolean>(false);
  message = signal<string>('');
  messageType = signal<'success' | 'error' | ''>('');

  constructor(
    private storyService: StoryService,
    private router: Router
  ) {}

  switchMode(mode: 'text' | 'audio') {
    this.inputMode.set(mode);
    this.message.set('');
  }

  async startRecording() {
    try {
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
      this.mediaRecorder = new MediaRecorder(stream);
      const audioChunks: Blob[] = [];

      this.mediaRecorder.ondataavailable = (event) => {
        audioChunks.push(event.data);
      };

      this.mediaRecorder.onstop = () => {
        const audioBlob = new Blob(audioChunks, { type: 'audio/wav' });
        this.audioBlob.set(audioBlob);
        this.audioUrl.set(URL.createObjectURL(audioBlob));
        stream.getTracks().forEach(track => track.stop());
      };

      this.mediaRecorder.start();
      this.isRecording.set(true);
      this.message.set('🎤 Đang ghi âm...');
      this.messageType.set('');
    } catch (error) {
      this.message.set('❌ Không thể truy cập microphone');
      this.messageType.set('error');
    }
  }

  stopRecording() {
    if (this.mediaRecorder && this.isRecording()) {
      this.mediaRecorder.stop();
      this.isRecording.set(false);
      this.message.set('✅ Đã ghi âm xong');
      this.messageType.set('success');
    }
  }

  clearAudio() {
    this.audioBlob.set(null);
    this.audioUrl.set('');
    this.message.set('');
  }

  async generateStory() {
    if (!this.childName()) {
      this.message.set('⚠️ Vui lòng nhập tên bé');
      this.messageType.set('error');
      return;
    }

    if (this.inputMode() === 'text' && !this.inputText()) {
      this.message.set('⚠️ Vui lòng nhập nội dung truyện');
      this.messageType.set('error');
      return;
    }

    if (this.inputMode() === 'audio' && !this.audioBlob()) {
      this.message.set('⚠️ Vui lòng ghi âm trước');
      this.messageType.set('error');
      return;
    }

    this.loading.set(true);
    this.message.set('✨ Đang tạo câu chuyện thần kỳ...');
    this.messageType.set('');

    try {
      if (this.inputMode() === 'text') {
        await this.generateFromText();
      } else {
        await this.generateFromAudio();
      }
    } catch (error: any) {
      this.loading.set(false);
      this.message.set('❌ ' + (error.message || 'Lỗi khi tạo truyện'));
      this.messageType.set('error');
    }
  }

  private async generateFromText() {
    const request: StoryRequest = {
      inputText: this.inputText(),
      childName: this.childName(),
      childAge: this.childAge(),
      theme: this.theme(),
      additionalContext: this.additionalContext(),
      pageCount: this.pageCount(),
      language: 'vi'
    };

    this.storyService.generateFromText(request).subscribe({
      next: (response) => {
        this.loading.set(false);
        if (response.success && response.story) {
          this.message.set('🎉 ' + response.message);
          this.messageType.set('success');
          setTimeout(() => {
            this.router.navigate(['/story', response.story!.id]);
          }, 1000);
        } else {
          this.message.set('❌ ' + response.message);
          this.messageType.set('error');
        }
      },
      error: (err) => {
        this.loading.set(false);
        this.message.set('❌ Lỗi: ' + (err.error?.message || err.message));
        this.messageType.set('error');
      }
    });
  }

  private async generateFromAudio() {
    if (!this.audioBlob()) return;

    const audioBase64 = await this.blobToBase64(this.audioBlob()!);

    const request: AudioStoryRequest = {
      audioBase64: audioBase64.split(',')[1], // Remove data:audio/wav;base64, prefix
      childName: this.childName(),
      childAge: this.childAge(),
      theme: this.theme(),
      pageCount: this.pageCount(),
      language: 'vi'
    };

    this.storyService.generateFromAudio(request).subscribe({
      next: (response) => {
        this.loading.set(false);
        if (response.success && response.story) {
          this.message.set('🎉 ' + response.message);
          this.messageType.set('success');
          setTimeout(() => {
            this.router.navigate(['/story', response.story!.id]);
          }, 1000);
        } else {
          this.message.set('❌ ' + response.message);
          this.messageType.set('error');
        }
      },
      error: (err) => {
        this.loading.set(false);
        this.message.set('❌ Lỗi: ' + (err.error?.message || err.message));
        this.messageType.set('error');
      }
    });
  }

  private blobToBase64(blob: Blob): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onloadend = () => resolve(reader.result as string);
      reader.onerror = reject;
      reader.readAsDataURL(blob);
    });
  }

  goToStories() {
    this.router.navigate(['/stories']);
  }
}

