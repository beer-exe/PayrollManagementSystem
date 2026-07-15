import React, { useState, useRef, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { useAuthStore } from '@/store/useAuthStore';
import { useThemeStore } from '@/store/useThemeStore';
import './Header.css';

interface HeaderProps {
  isCollapsed: boolean;
  toggleCollapse: () => void;
  toggleMobileSidebar: () => void;
}

export const Header: React.FC<HeaderProps> = ({ isCollapsed, toggleCollapse, toggleMobileSidebar }) => {
  const { user, logout } = useAuthStore();
  const { theme, toggleTheme } = useThemeStore();
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

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
        <button onClick={toggleMobileSidebar} className="header-toggle-btn header-toggle-btn--mobile" aria-label="Mở menu">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" d="M3.75 6.75h16.5M3.75 12h16.5m-16.5 5.25H12" />
          </svg>
        </button>

        {/* Nút Thu gọn/Mở rộng cho Desktop */}
        <button onClick={toggleCollapse} className="header-toggle-btn header-toggle-btn--desktop" aria-label="Thu gọn menu">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className={`header-toggle-icon ${isCollapsed ? 'collapsed' : ''}`}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M3.75 6.75h16.5M3.75 12h16.5m-16.5 5.25h16.5" />
          </svg>
        </button>

        <div className="header-breadcrumb">
          <span className="header-breadcrumb-text">Dashboard</span>
        </div>
      </div>

      <div className="header-right">
        <button 
          onClick={toggleTheme}
          className="header-theme-toggle"
          aria-label="Toggle Dark Mode"
        >
          {theme === 'light' ? (
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" d="M21.752 15.002A9.72 9.72 0 0 1 18 15.75c-5.385 0-9.75-4.365-9.75-9.75 0-1.33.266-2.597.748-3.752A9.753 9.753 0 0 0 3 11.25C3 16.635 7.365 21 12.75 21a9.753 9.753 0 0 0 9.002-5.998Z" />
            </svg>
          ) : (
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" d="M12 3v2.25m6.364.386-1.591 1.591M21 12h-2.25m-.386 6.364-1.591-1.591M12 18.75V21m-4.773-4.227-1.591 1.591M5.25 12H3m4.227-4.773L5.636 5.636M15.75 12a3.75 3.75 0 1 1-7.5 0 3.75 3.75 0 0 1 7.5 0Z" />
            </svg>
          )}
        </button>

        <div className="header-user-wrapper" ref={dropdownRef}>
          <button 
            className="header-user-btn" 
            onClick={() => setIsDropdownOpen(!isDropdownOpen)}
            aria-expanded={isDropdownOpen}
          >
            <div className="header-user-avatar">
              {getInitials(user?.name || 'User')}
            </div>
            <div className="header-user-info">
              <span className="header-user-name">{user?.name || 'Tài khoản'}</span>
              <span className="header-user-role">{user?.email || 'Admin'}</span>
            </div>
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className={`header-user-chevron ${isDropdownOpen ? 'open' : ''}`}>
              <path strokeLinecap="round" strokeLinejoin="round" d="m19.5 8.25-7.5 7.5-7.5-7.5" />
            </svg>
          </button>

          {isDropdownOpen && (
            <div className="header-dropdown-menu">
              <div className="header-dropdown-header">
                <p className="header-dropdown-title">Đăng nhập với tư cách</p>
                <p className="header-dropdown-email">{user?.email}</p>
              </div>
              <ul className="header-dropdown-list">
                <li>
                  <Link to="/dashboard/ho-so" className="header-dropdown-item" onClick={() => setIsDropdownOpen(false)}>
                    Hồ sơ cá nhân
                  </Link>
                </li>
                <li>
                  <button className="header-dropdown-item">Đổi mật khẩu</button>
                </li>
                <li>
                  <button onClick={handleLogout} className="header-dropdown-item header-dropdown-item--danger">
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