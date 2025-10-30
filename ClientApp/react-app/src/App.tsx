import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import CreateStory from './pages/CreateStory';
import StoryList from './pages/StoryList';
import StoryViewer from './pages/StoryViewer';
import './App.css';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Navigate to="/create" replace />} />
        <Route path="/create" element={<CreateStory />} />
        <Route path="/stories" element={<StoryList />} />
        <Route path="/story/:id" element={<StoryViewer />} />
        <Route path="*" element={<Navigate to="/create" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
