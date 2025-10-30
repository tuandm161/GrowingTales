import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { storyService } from '../services/story.service';
import type { Story } from '../models/story.model';
import './StoryList.css';

export default function StoryList() {
  const navigate = useNavigate();
  const [stories, setStories] = useState<Story[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  
  useEffect(() => {
    loadStories();
  }, []);
  
  const loadStories = async () => {
    setLoading(true);
    try {
      const data = await storyService.getAllStories();
      setStories(data);
      setLoading(false);
    } catch (err: any) {
      setError('Không thể tải danh sách truyện: ' + err.message);
      setLoading(false);
    }
  };
  
  const viewStory = (storyId: string) => {
    navigate(`/story/${storyId}`);
  };
  
  const createNewStory = () => {
    navigate('/create');
  };
  
  const deleteStory = async (story: Story, event: React.MouseEvent) => {
    event.stopPropagation();
    
    if (window.confirm(`Bạn có chắc muốn xóa câu chuyện "${story.title}"?`)) {
      try {
        await storyService.deleteStory(story.id);
        setStories(stories.filter(s => s.id !== story.id));
      } catch (err: any) {
        alert('Lỗi khi xóa: ' + err.message);
      }
    }
  };
  
  const formatDate = (dateString: string): string => {
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  };
  
  return (
    <div className="list-container">
      <div className="header">
        <h1>📚 Thư Viện Câu Chuyện</h1>
        <p className="subtitle">Tất cả những câu chuyện tuyệt vời bạn đã tạo</p>
        <div className="header-actions">
          <button onClick={createNewStory} className="btn btn-primary">
            ✨ Tạo Truyện Mới
          </button>
        </div>
      </div>
      
      <div className="content">
        {loading && (
          <div className="loading">
            <div className="spinner"></div>
            <p>Đang tải danh sách truyện...</p>
          </div>
        )}
        
        {error && !loading && (
          <div className="error-message">
            <h3>❌ Lỗi</h3>
            <p>{error}</p>
            <button onClick={loadStories} className="btn btn-primary">
              Thử lại
            </button>
          </div>
        )}
        
        {!loading && !error && stories.length === 0 && (
          <div className="empty-state">
            <div className="empty-icon">📖</div>
            <h2>Chưa có câu chuyện nào</h2>
            <p>Hãy tạo câu chuyện đầu tiên cho bé yêu của bạn!</p>
            <button onClick={createNewStory} className="btn btn-primary btn-large">
              ✨ Tạo Câu Chuyện Đầu Tiên
            </button>
          </div>
        )}
        
        {!loading && stories.length > 0 && (
          <div className="stories-grid">
            {stories.map((story) => (
              <div key={story.id} className="story-card" onClick={() => viewStory(story.id)}>
                <div className="card-header">
                  <div className="card-icon">📖</div>
                  <button 
                    onClick={(e) => deleteStory(story, e)} 
                    className="delete-btn"
                    title="Xóa câu chuyện">
                    🗑️
                  </button>
                </div>
                
                <div className="card-body">
                  <h3 className="story-title">{story.title}</h3>
                  <p className="story-description">{story.description}</p>
                  
                  <div className="story-meta">
                    <div className="meta-item">
                      <span className="meta-icon">👶</span>
                      <span className="meta-text">{story.childName}</span>
                    </div>
                    <div className="meta-item">
                      <span className="meta-icon">🎂</span>
                      <span className="meta-text">{story.childAge} tuổi</span>
                    </div>
                    {story.theme && (
                      <div className="meta-item">
                        <span className="meta-icon">🌟</span>
                        <span className="meta-text">{story.theme}</span>
                      </div>
                    )}
                    <div className="meta-item">
                      <span className="meta-icon">📄</span>
                      <span className="meta-text">{story.pages.length} trang</span>
                    </div>
                  </div>
                </div>
                
                <div className="card-footer">
                  <span className="date">{formatDate(story.createdAt)}</span>
                  <span className="read-link">Đọc truyện →</span>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

