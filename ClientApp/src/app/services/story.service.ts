import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  Story,
  StoryRequest,
  AudioStoryRequest,
  StoryResponse
} from '../models/story.model';

@Injectable({
  providedIn: 'root'
})
export class StoryService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // Story generation endpoints
  generateFromText(request: StoryRequest): Observable<StoryResponse> {
    return this.http.post<StoryResponse>(`${this.apiUrl}/story/generate-from-text`, request);
  }

  generateFromAudio(request: AudioStoryRequest): Observable<StoryResponse> {
    return this.http.post<StoryResponse>(`${this.apiUrl}/story/generate-from-audio`, request);
  }

  // Story management endpoints
  getAllStories(): Observable<Story[]> {
    return this.http.get<Story[]>(`${this.apiUrl}/story`);
  }

  getStory(id: string): Observable<Story> {
    return this.http.get<Story>(`${this.apiUrl}/story/${id}`);
  }

  getStoriesByChild(childName: string): Observable<Story[]> {
    return this.http.get<Story[]>(`${this.apiUrl}/story/by-child/${childName}`);
  }

  deleteStory(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/story/${id}`);
  }
}

