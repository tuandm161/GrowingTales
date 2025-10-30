import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { storyService } from '../services/story.service';
import type { StoryRequest, AudioStoryRequest } from '../models/story.model';
import './CreateStory.css';

type InputMode = 'text' | 'audio';
type MessageType = 'success' | 'error' | '';

export default function CreateStory() {
  const navigate = useNavigate();
  
  // Form fields
  const [inputMode, setInputMode] = useState<InputMode>('text');
  const [inputText, setInputText] = useState('');
  const [childName, setChildName] = useState('');
  const [childAge, setChildAge] = useState(5);
  const [theme, setTheme] = useState('');
  const [additionalContext, setAdditionalContext] = useState('');
  const [pageCount, setPageCount] = useState(5);
  
  // Audio recording
  const [isRecording, setIsRecording] = useState(false);
  const [audioBlob, setAudioBlob] = useState<Blob | null>(null);
  const [audioUrl, setAudioUrl] = useState('');
  const [mediaRecorder, setMediaRecorder] = useState<MediaRecorder | null>(null);
  
  // UI state
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');
  const [messageType, setMessageType] = useState<MessageType>('');
  
  const switchMode = (mode: InputMode) => {
    setInputMode(mode);
    setMessage('');
  };
  
  const startRecording = async () => {
    try {
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
      const recorder = new MediaRecorder(stream);
      const audioChunks: Blob[] = [];
      
      recorder.ondataavailable = (event) => {
        audioChunks.push(event.data);
      };
      
      recorder.onstop = () => {
        const blob = new Blob(audioChunks, { type: 'audio/wav' });
        setAudioBlob(blob);
        setAudioUrl(URL.createObjectURL(blob));
        stream.getTracks().forEach(track => track.stop());
      };
      
      recorder.start();
      setMediaRecorder(recorder);
      setIsRecording(true);
      setMessage('🎤 Đang ghi âm...');
      setMessageType('');
    } catch (error) {
      setMessage('❌ Không thể truy cập microphone');
      setMessageType('error');
    }
  };
  
  const stopRecording = () => {
    if (mediaRecorder && isRecording) {
      mediaRecorder.stop();
      setIsRecording(false);
      setMessage('✅ Đã ghi âm xong');
      setMessageType('success');
    }
  };
  
  const clearAudio = () => {
    setAudioBlob(null);
    setAudioUrl('');
    setMessage('');
  };
  
  const generateStory = async () => {
    if (!childName) {
      setMessage('⚠️ Vui lòng nhập tên bé');
      setMessageType('error');
      return;
    }
    
    if (inputMode === 'text' && !inputText) {
      setMessage('⚠️ Vui lòng nhập nội dung truyện');
      setMessageType('error');
      return;
    }
    
    if (inputMode === 'audio' && !audioBlob) {
      setMessage('⚠️ Vui lòng ghi âm trước');
      setMessageType('error');
      return;
    }
    
    setLoading(true);
    setMessage('✨ Đang tạo câu chuyện thần kỳ...');
    setMessageType('');
    
    try {
      if (inputMode === 'text') {
        await generateFromText();
      } else {
        await generateFromAudio();
      }
    } catch (error: any) {
      setLoading(false);
      setMessage('❌ ' + (error.message || 'Lỗi khi tạo truyện'));
      setMessageType('error');
    }
  };
  
  const generateFromText = async () => {
    const request: StoryRequest = {
      inputText,
      childName,
      childAge,
      theme,
      additionalContext,
      pageCount,
      language: 'vi'
    };
    
    try {
      const response = await storyService.generateFromText(request);
      setLoading(false);
      if (response.success && response.story) {
        setMessage('🎉 ' + response.message);
        setMessageType('success');
        setTimeout(() => {
          navigate(`/story/${response.story!.id}`);
        }, 1000);
      } else {
        setMessage('❌ ' + response.message);
        setMessageType('error');
      }
    } catch (err: any) {
      setLoading(false);
      setMessage('❌ Lỗi: ' + (err.response?.data?.message || err.message));
      setMessageType('error');
    }
  };
  
  const generateFromAudio = async () => {
    if (!audioBlob) return;
    
    const audioBase64 = await blobToBase64(audioBlob);
    
    const request: AudioStoryRequest = {
      audioBase64: audioBase64.split(',')[1], // Remove data:audio/wav;base64, prefix
      childName,
      childAge,
      theme,
      pageCount,
      language: 'vi'
    };
    
    try {
      const response = await storyService.generateFromAudio(request);
      setLoading(false);
      if (response.success && response.story) {
        setMessage('🎉 ' + response.message);
        setMessageType('success');
        setTimeout(() => {
          navigate(`/story/${response.story!.id}`);
        }, 1000);
      } else {
        setMessage('❌ ' + response.message);
        setMessageType('error');
      }
    } catch (err: any) {
      setLoading(false);
      setMessage('❌ Lỗi: ' + (err.response?.data?.message || err.message));
      setMessageType('error');
    }
  };
  
  const blobToBase64 = (blob: Blob): Promise<string> => {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onloadend = () => resolve(reader.result as string);
      reader.onerror = reject;
      reader.readAsDataURL(blob);
    });
  };
  
  const goToStories = () => {
    navigate('/stories');
  };
  
  return (
    <div className="create-container">
      <div className="create-card">
        <div className="header">
          <h1>✨ Tạo Câu Chuyện Kỳ Diệu</h1>
          <p className="subtitle">Biến giọng nói hoặc văn bản thành câu chuyện đẹp cho bé yêu</p>
        </div>
        
        <div className="content">
          {/* Mode Selection */}
          <div className="mode-selector">
            <button 
              className={`mode-btn ${inputMode === 'text' ? 'active' : ''}`}
              onClick={() => switchMode('text')}>
              📝 Nhập Văn Bản
            </button>
            <button 
              className={`mode-btn ${inputMode === 'audio' ? 'active' : ''}`}
              onClick={() => switchMode('audio')}>
              🎤 Ghi Âm
            </button>
          </div>
          
          {/* Text Input Mode */}
          {inputMode === 'text' && (
            <div className="input-section">
              <label>Nội dung câu chuyện:</label>
              <textarea
                value={inputText}
                onChange={(e) => setInputText(e.target.value)}
                placeholder="Nhập nội dung câu chuyện hoặc ý tưởng bạn muốn kể cho bé..."
                rows={6}
                className="text-input"
                disabled={loading}
              />
            </div>
          )}
          
          {/* Audio Input Mode */}
          {inputMode === 'audio' && (
            <div className="input-section">
              <label>Ghi âm câu chuyện:</label>
              <div className="audio-controls">
                {!isRecording && !audioBlob && (
                  <button onClick={startRecording} className="btn btn-record">
                    🎤 Bắt Đầu Ghi Âm
                  </button>
                )}
                
                {isRecording && (
                  <>
                    <button onClick={stopRecording} className="btn btn-stop">
                      ⏹️ Dừng Ghi Âm
                    </button>
                    <span className="recording-indicator">🔴 Đang ghi âm...</span>
                  </>
                )}
                
                {audioBlob && !isRecording && (
                  <div className="audio-preview">
                    <audio src={audioUrl} controls className="audio-player" />
                    <button onClick={clearAudio} className="btn btn-clear">
                      🗑️ Xóa & Ghi Lại
                    </button>
                  </div>
                )}
              </div>
            </div>
          )}
          
          {/* Story Details */}
          <div className="form-section">
            <h3>📋 Thông Tin Câu Chuyện</h3>
            
            <div className="form-row">
              <div className="form-group">
                <label>Tên bé: *</label>
                <input
                  type="text"
                  value={childName}
                  onChange={(e) => setChildName(e.target.value)}
                  placeholder="VD: Minh An"
                  className="form-input"
                  disabled={loading}
                />
              </div>
              
              <div className="form-group">
                <label>Tuổi: *</label>
                <input
                  type="number"
                  value={childAge}
                  onChange={(e) => setChildAge(parseInt(e.target.value))}
                  min={1}
                  max={18}
                  className="form-input"
                  disabled={loading}
                />
              </div>
            </div>
            
            <div className="form-group">
              <label>Chủ đề:</label>
              <input
                type="text"
                value={theme}
                onChange={(e) => setTheme(e.target.value)}
                placeholder="VD: Phiêu lưu, Gia đình, Tình bạn, Động vật..."
                className="form-input"
                disabled={loading}
              />
            </div>
            
            <div className="form-group">
              <label>Bối cảnh thêm (tuỳ chọn):</label>
              <textarea
                value={additionalContext}
                onChange={(e) => setAdditionalContext(e.target.value)}
                placeholder="VD: Bé thích khủng long, bé mới học được bài thơ, bé vừa đi chơi công viên..."
                rows={3}
                className="form-input"
                disabled={loading}
              />
            </div>
            
            <div className="form-group">
              <label>Số trang: {pageCount}</label>
              <input
                type="range"
                value={pageCount}
                onChange={(e) => setPageCount(parseInt(e.target.value))}
                min={3}
                max={10}
                className="range-input"
                disabled={loading}
              />
              <div className="range-labels">
                <span>3 trang</span>
                <span>10 trang</span>
              </div>
            </div>
          </div>
          
          {/* Message */}
          {message && (
            <div className={`message ${messageType}`}>
              {message}
            </div>
          )}
          
          {/* Actions */}
          <div className="button-group">
            <button 
              onClick={generateStory}
              disabled={loading || !childName}
              className="btn btn-primary btn-large">
              {loading ? (
                <span>⏳ Đang tạo câu chuyện...</span>
              ) : (
                <span>✨ Tạo Câu Chuyện</span>
              )}
            </button>
          </div>
          
          <div className="secondary-actions">
            <button onClick={goToStories} className="btn btn-link">
              📚 Xem truyện đã tạo
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}

