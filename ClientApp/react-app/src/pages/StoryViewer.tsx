import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { storyService } from '../services/story.service';
import type { Story, StoryPage } from '../models/story.model';
import './StoryViewer.css';

export default function StoryViewer() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  
  const [story, setStory] = useState<Story | null>(null);
  const [currentPage, setCurrentPage] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  
  useEffect(() => {
    if (id) {
      loadStory(id);
    } else {
      setError('Không tìm thấy ID câu chuyện');
      setLoading(false);
    }
  }, [id]);
  
  const loadStory = async (storyId: string) => {
    setLoading(true);
    try {
      const data = await storyService.getStory(storyId);
      setStory(data);
      setLoading(false);
    } catch (err: any) {
      setError('Không thể tải câu chuyện: ' + err.message);
      setLoading(false);
    }
  };
  
  const currentStoryPage = (): StoryPage | null => {
    if (!story || !story.pages || story.pages.length === 0) return null;
    return story.pages[currentPage] || null;
  };
  
  const totalPages = story?.pages?.length || 0;
  const isFirstPage = currentPage === 0;
  const isLastPage = currentPage === totalPages - 1;
  
  const nextPage = () => {
    if (!isLastPage) {
      setCurrentPage(currentPage + 1);
    }
  };
  
  const previousPage = () => {
    if (!isFirstPage) {
      setCurrentPage(currentPage - 1);
    }
  };
  
  const goToPage = (pageIndex: number) => {
    if (pageIndex >= 0 && pageIndex < totalPages) {
      setCurrentPage(pageIndex);
    }
  };
  
  const goToStories = () => {
    navigate('/stories');
  };
  
  const createNewStory = () => {
    navigate('/create');
  };
  
  const deleteStory = async () => {
    if (!story) return;
    
    if (window.confirm('Bạn có chắc muốn xóa câu chuyện này?')) {
      try {
        await storyService.deleteStory(story.id);
        navigate('/stories');
      } catch (err: any) {
        alert('Lỗi khi xóa: ' + err.message);
      }
    }
  };
  
  const printStory = () => {
    window.print();
  };
  
  return (
    <div className="viewer-container">
      {loading && (
        <div className="loading">
          <div className="spinner"></div>
          <p>Đang tải câu chuyện...</p>
        </div>
      )}
      
      {error && !loading && (
        <div className="error-container">
          <div className="error-card">
            <h2>❌ Lỗi</h2>
            <p>{error}</p>
            <button onClick={goToStories} className="btn btn-primary">
              Quay lại danh sách
            </button>
          </div>
        </div>
      )}
      
      {story && !loading && (
        <div className="storybook">
          {/* Cover Page */}
          {isFirstPage && (
            <div className="book-cover">
              <div className="cover-content">
                <div className="cover-decoration">✨</div>
                <h1 className="story-title">{story.title}</h1>
                <p className="story-subtitle">{story.description}</p>
                <div className="cover-info">
                  <p className="child-info">📖 Dành tặng: <strong>{story.childName}</strong></p>
                  <p className="age-info">🎂 Tuổi: <strong>{story.childAge}</strong></p>
                  {story.theme && (
                    <p className="theme-info">🌟 Chủ đề: <strong>{story.theme}</strong></p>
                  )}
                </div>
                <div className="cover-decoration-bottom">📚</div>
              </div>
            </div>
          )}
          
          {/* Story Pages */}
          {!isFirstPage && (
            <div className="story-page">
              <div className="page-number">Trang {currentPage}</div>
              
              <div className="page-content">
                {/* Image */}
                <div className="page-image">
                  {currentStoryPage()?.imageUrl ? (
                    <img 
                      src={currentStoryPage()!.imageUrl} 
                      alt={`Trang ${currentPage}`}
                      className="story-image" 
                    />
                  ) : (
                    <div className="image-placeholder">
                      <span className="placeholder-icon">🎨</span>
                      <p className="image-prompt">{currentStoryPage()?.imagePrompt}</p>
                    </div>
                  )}
                </div>
                
                {/* Story Text */}
                <div className="page-text">
                  <p>{currentStoryPage()?.content}</p>
                </div>
              </div>
            </div>
          )}
          
          {/* Navigation Controls */}
          <div className="navigation">
            <button 
              onClick={previousPage} 
              disabled={isFirstPage}
              className="nav-btn nav-prev">
              ← Trang trước
            </button>
            
            <div className="page-dots">
              {story.pages.map((_, i) => (
                <button
                  key={i}
                  onClick={() => goToPage(i)}
                  className={`dot-btn ${currentPage === i ? 'active' : ''}`}
                  title={`Trang ${i + 1}`}>
                </button>
              ))}
            </div>
            
            <button 
              onClick={nextPage} 
              disabled={isLastPage}
              className="nav-btn nav-next">
              Trang sau →
            </button>
          </div>
          
          {/* Action Buttons */}
          <div className="actions">
            <button onClick={goToStories} className="btn btn-secondary">
              📚 Danh sách truyện
            </button>
            <button onClick={createNewStory} className="btn btn-primary">
              ✨ Tạo truyện mới
            </button>
            <button onClick={printStory} className="btn btn-secondary">
              🖨️ In truyện
            </button>
            <button onClick={deleteStory} className="btn btn-danger">
              🗑️ Xóa
            </button>
          </div>
        </div>
      )}
    </div>
  );
}

