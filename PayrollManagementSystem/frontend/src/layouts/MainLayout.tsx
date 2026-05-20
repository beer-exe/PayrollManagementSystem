import React, { useState, useEffect } from 'react';
import { Outlet, useLocation } from 'react-router-dom';
import { Sidebar } from './components/Sidebar';
import { Header } from './components/Header';
import './MainLayout.css';

export const MainLayout: React.FC = () => {
  const [isSidebarCollapsed, setIsSidebarCollapsed] = useState(false);
  const [isMobileSidebarOpen, setIsMobileSidebarOpen] = useState(false);
  const location = useLocation();

  // Đóng sidebar trên mobile mỗi khi chuyển route
  useEffect(() => {
    setIsMobileSidebarOpen(false);
  }, [location.pathname]);

  return (
    <div className="main-layout-container">
      {/* Sidebar Component */}
      <Sidebar 
        isCollapsed={isSidebarCollapsed} 
        isOpenMobile={isMobileSidebarOpen}
        onCloseMobile={() => setIsMobileSidebarOpen(false)}
      />

      {/* Main Content Wrapper */}
      <div className={`main-content-wrapper ${isSidebarCollapsed ? 'collapsed' : ''}`}>
        <Header 
          isCollapsed={isSidebarCollapsed}
          toggleCollapse={() => setIsSidebarCollapsed(!isSidebarCollapsed)}
          toggleMobileSidebar={() => setIsMobileSidebarOpen(true)}
        />
        
        {/* Vùng nội dung chính */}
        <main className="main-content-area">
          <Outlet />
        </main>
      </div>
    </div>
  );
};