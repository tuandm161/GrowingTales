import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { StoryService } from '../../services/story.service';
import { Story } from '../../models/story.model';

@Component({
  selector: 'app-story-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './story-list.component.html',
  styleUrl: './story-list.component.css'
})
export class StoryListComponent implements OnInit {
  stories = signal<Story[]>([]);
  loading = signal<boolean>(true);
  error = signal<string>('');

  constructor(
    private storyService: StoryService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadStories();
  }

  loadStories() {
    this.loading.set(true);
    this.storyService.getAllStories().subscribe({
      next: (stories) => {
        this.stories.set(stories);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Không thể tải danh sách truyện: ' + err.message);
        this.loading.set(false);
      }
    });
  }

  viewStory(storyId: string) {
    this.router.navigate(['/story', storyId]);
  }

  createNewStory() {
    this.router.navigate(['/create']);
  }

  deleteStory(story: Story, event: Event) {
    event.stopPropagation();
    
    if (confirm(`Bạn có chắc muốn xóa câu chuyện "${story.title}"?`)) {
      this.storyService.deleteStory(story.id).subscribe({
        next: () => {
          this.stories.set(this.stories().filter(s => s.id !== story.id));
        },
        error: (err) => {
          alert('Lỗi khi xóa: ' + err.message);
        }
      });
    }
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }
}

