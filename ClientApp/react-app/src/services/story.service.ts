import axios from 'axios';
import { environment } from '../config/environment';
import type {
  Story,
  StoryRequest,
  AudioStoryRequest,
  StoryResponse
} from '../models/story.model';

const api = axios.create({
  baseURL: environment.apiUrl,
  headers: {
    'Content-Type': 'application/json'
  }
});

export const storyService = {
  // Story generation endpoints
  generateFromText: async (request: StoryRequest): Promise<StoryResponse> => {
    const response = await api.post<StoryResponse>('/story/generate-from-text', request);
    return response.data;
  },

  generateFromAudio: async (request: AudioStoryRequest): Promise<StoryResponse> => {
    const response = await api.post<StoryResponse>('/story/generate-from-audio', request);
    return response.data;
  },

  // Story management endpoints
  getAllStories: async (): Promise<Story[]> => {
    const response = await api.get<Story[]>('/story');
    return response.data;
  },

  getStory: async (id: string): Promise<Story> => {
    const response = await api.get<Story>(`/story/${id}`);
    return response.data;
  },

  getStoriesByChild: async (childName: string): Promise<Story[]> => {
    const response = await api.get<Story[]>(`/story/by-child/${childName}`);
    return response.data;
  },

  deleteStory: async (id: string): Promise<any> => {
    const response = await api.delete(`/story/${id}`);
    return response.data;
  }
};

