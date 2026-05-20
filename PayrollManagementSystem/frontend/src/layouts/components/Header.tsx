import React, { useState, useRef, useEffect } from 'react';
import { useAuthStore } from '@/store/useAuthStore';
import './Header.css';

interface HeaderProps {
  isCollapsed: boolean;
  toggleCollapse: () => void;
  toggleMobileSidebar: () => void;
}

export const Header: React.FC<HeaderProps> = ({ isCollapsed, toggleCollapse, toggleMobileSidebar }) => {
  const { user, logout } = useAuthStore();
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  // Tạo Avatar từ chữ cái đầu của tên (Ví dụ: Nguyễn Văn A -> NA)
  const getInitials = (name: string) => {
    if (!name) return 'U';
    const parts = name.trim().split(' ');
    if (parts.length === 1) return parts[0].charAt(0).toUpperCase();
    return (parts[0].charAt(0) + parts[parts.length - 1].charAt(0)).toUpperCase();
  };

  const handleLogout = () => {
    logout();
    window.location.href = '/login';
  };

  // Đóng dropdown khi click ra ngoài
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
        setIsDropdownOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  return (
    <header className="header-container">
      <div className="header-left">
        {/* Nút Hamburger cho Mobile */}
        <button onClick={toggleMobileSidebar} className="toggle-btn lg:hidden" aria-label="Mở menu">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-6 h-6"><path strokeLinecap="round" strokeLinejoin="round" d="M3.75 6.75h16.5M3.75 12h16.5m-16.5 5.25H12" /></svg>
        </button>

        {/* Nút Thu gọn/Mở rộng cho Desktop */}
        <button onClick={toggleCollapse} className="toggle-btn hidden lg:flex" aria-label="Thu gọn menu">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className={`w-5 h-5 transition-transform duration-300 ${isCollapsed ? 'rotate-180' : ''}`}><path strokeLinecap="round" strokeLinejoin="round" d="M3.75 6.75h16.5M3.75 12h16.5m-16.5 5.25h16.5" /></svg>
        </button>

        <div className="header-breadcrumb">
          {/* Có thể tích hợp react-router logic để render breadcrumb thật ở đây */}
          <span className="font-semibold text-gray-800 dark:text-gray-100">Bảng điều khiển</span>
        </div>
      </div>

      <div className="header-right">
        <div className="relative" ref={dropdownRef}>
          <button 
            className="user-dropdown-btn" 
            onClick={() => setIsDropdownOpen(!isDropdownOpen)}
            aria-expanded={isDropdownOpen}
          >
            <div className="user-avatar">
              {getInitials(user?.name || 'User')}
            </div>
            <div className="user-info">
              <span className="user-name">{user?.name || 'Tài khoản'}</span>
              <span className="user-role">{user?.email || 'Admin'}</span>
            </div>
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className={`w-4 h-4 text-gray-500 transition-transform ${isDropdownOpen ? 'rotate-180' : ''}`}><path strokeLinecap="round" strokeLinejoin="round" d="m19.5 8.25-7.5 7.5-7.5-7.5" /></svg>
          </button>

          {isDropdownOpen && (
            <div className="dropdown-menu">
              <div className="px-4 py-3 border-b border-gray-100 dark:border-gray-700">
                <p className="text-sm text-gray-500 dark:text-gray-400">Đăng nhập với tư cách</p>
                <p className="text-sm font-medium text-gray-900 dark:text-white truncate">{user?.email}</p>
              </div>
              <ul className="py-1">
                <li>
                  <button className="dropdown-item">Hồ sơ cá nhân</button>
                </li>
                <li>
                  <button className="dropdown-item">Đổi mật khẩu</button>
                </li>
                <li>
                  <button onClick={handleLogout} className="dropdown-item text-red-600 hover:text-red-700 hover:bg-red-50 dark:hover:bg-red-900/20">
                    Đăng xuất
                  </button>
                </li>
              </ul>
            </div>
          )}
        </div>
      </div>
    </header>
  );
};