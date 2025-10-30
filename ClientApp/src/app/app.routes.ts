import { Routes } from '@angular/router';
import { CreateStoryComponent } from './components/create-story/create-story.component';
import { StoryViewerComponent } from './components/story-viewer/story-viewer.component';
import { StoryListComponent } from './components/story-list/story-list.component';

export const routes: Routes = [
  { path: '', redirectTo: '/create', pathMatch: 'full' },
  { path: 'create', component: CreateStoryComponent },
  { path: 'stories', component: StoryListComponent },
  { path: 'story/:id', component: StoryViewerComponent },
  { path: '**', redirectTo: '/create' }
];
