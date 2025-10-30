import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { StoryService } from '../../services/story.service';
import { Story, StoryPage } from '../../models/story.model';

@Component({
  selector: 'app-story-viewer',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './story-viewer.component.html',
  styleUrl: './story-viewer.component.css'
})
export class StoryViewerComponent implements OnInit {
  story = signal<Story | null>(null);
  currentPage = signal<number>(0);
  loading = signal<boolean>(true);
  error = signal<string>('');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private storyService: StoryService
  ) {}

  ngOnInit() {
    const storyId = this.route.snapshot.paramMap.get('id');
    if (storyId) {
      this.loadStory(storyId);
    } else {
      this.error.set('Không tìm thấy ID câu chuyện');
      this.loading.set(false);
    }
  }

  loadStory(id: string) {
    this.loading.set(true);
    this.storyService.getStory(id).subscribe({
      next: (story) => {
        this.story.set(story);
        this.loading.set(false);
      },
      error: (err) => {
        this.error.set('Không thể tải câu chuyện: ' + err.message);
        this.loading.set(false);
      }
    });
  }

  get currentStoryPage(): StoryPage | null {
    const story = this.story();
    if (!story || !story.pages || story.pages.length === 0) return null;
    return story.pages[this.currentPage()] || null;
  }

  get totalPages(): number {
    return this.story()?.pages?.length || 0;
  }

  get isFirstPage(): boolean {
    return this.currentPage() === 0;
  }

  get isLastPage(): boolean {
    return this.currentPage() === this.totalPages - 1;
  }

  nextPage() {
    if (!this.isLastPage) {
      this.currentPage.set(this.currentPage() + 1);
    }
  }

  previousPage() {
    if (!this.isFirstPage) {
      this.currentPage.set(this.currentPage() - 1);
    }
  }

  goToPage(pageIndex: number) {
    if (pageIndex >= 0 && pageIndex < this.totalPages) {
      this.currentPage.set(pageIndex);
    }
  }

  goToStories() {
    this.router.navigate(['/stories']);
  }

  createNewStory() {
    this.router.navigate(['/create']);
  }

  deleteStory() {
    if (!this.story()) return;
    
    if (confirm('Bạn có chắc muốn xóa câu chuyện này?')) {
      this.storyService.deleteStory(this.story()!.id).subscribe({
        next: () => {
          this.router.navigate(['/stories']);
        },
        error: (err) => {
          alert('Lỗi khi xóa: ' + err.message);
        }
      });
    }
  }

  printStory() {
    window.print();
  }
}

