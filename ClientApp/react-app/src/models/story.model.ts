export interface Story {
  id: string;
  title: string;
  description: string;
  pages: StoryPage[];
  createdAt: string;
  childName: string;
  childAge: number;
  theme: string;
  coverImageUrl: string;
}

export interface StoryPage {
  pageNumber: number;
  content: string;
  imagePrompt: string;
  imageUrl: string;
}

export interface StoryRequest {
  inputText: string;
  childName: string;
  childAge: number;
  theme: string;
  additionalContext: string;
  pageCount: number;
  language: string;
}

export interface AudioStoryRequest {
  audioBase64: string;
  childName: string;
  childAge: number;
  theme: string;
  pageCount: number;
  language: string;
}

export interface StoryResponse {
  success: boolean;
  message: string;
  story?: Story;
}

