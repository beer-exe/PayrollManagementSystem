import React, { useState, useEffect } from "react";
import { NavLink, useLocation } from "react-router-dom";
import { useAuthStore } from "@/store/useAuthStore";
import "./Sidebar.css";

interface SidebarProps {
  isCollapsed: boolean;
  isOpenMobile: boolean;
  onCloseMobile: () => void;
}

interface MenuItem {
  path?: string;
  label: string;
  allowedRoles: string[];
  icon: React.ReactNode;
  children?: { path: string; label: string; allowedRoles: string[] }[];
}

const menuItems: MenuItem[] = [
  {
    path: "/profile",
    label: "Hồ sơ cá nhân",
    allowedRoles: ["Admin", "HR", "Employee"],
    icon: (
      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" d="M15.75 6a3.75 3.75 0 1 1-7.5 0 3.75 3.75 0 0 1 7.5 0ZM4.501 20.118a7.5 7.5 0 0 1 14.998 0A17.933 17.933 0 0 1 12 21.75c-2.676 0-5.216-.584-7.499-1.632Z" />
      </svg>
    ),
  },
  {
    path: "/admin/tai-khoan",
    label: "Quản lý tài khoản",
    allowedRoles: ["Admin"],
    icon: (
      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" d="M15 19.128a9.38 9.38 0 0 0 2.625.372 9.337 9.337 0 0 0 4.121-.952 4.125 4.125 0 0 0-7.533-2.493M15 19.128v-.003c0-1.113-.285-2.16-.786-3.07M15 19.128v.106A12.318 12.318 0 0 1 8.624 21c-2.331 0-4.512-.645-6.374-1.766l-.001-.109a6.375 6.375 0 0 1 11.964-3.07M12 6.375a3.375 3.375 0 1 1-6.75 0 3.375 3.375 0 0 1 6.75 0Zm8.25 2.25a2.625 2.625 0 1 1-5.25 0 2.625 2.625 0 0 1 5.25 0Z" />
      </svg>
    ),
  },
  {
    path: "/hr/nhan-vien",
    label: "Quản lý nhân viên",
    allowedRoles: ["HR"],
    icon: (
      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" d="M18 18.72a9.094 9.094 0 0 0 3.741-.479 3 3 0 0 0-4.682-2.72m.94 3.198.001.031c0 .225-.012.447-.037.666A11.944 11.944 0 0 1 12 21c-2.17 0-4.207-.576-5.963-1.584A6.062 6.062 0 0 1 6 18.719m12 0a5.971 5.971 0 0 0-.941-3.197m0 0A5.995 5.995 0 0 0 12 12.75a5.995 5.995 0 0 0-5.058 2.772m0 0a3 3 0 0 0-4.681 2.72 8.986 8.986 0 0 0 3.74.477m.94-3.197a5.971 5.971 0 0 0-.94 3.197M15 6.75a3 3 0 1 1-6 0 3 3 0 0 1 6 0Zm6 3a2.25 2.25 0 1 1-4.5 0 2.25 2.25 0 0 1 4.5 0Zm-13.5 0a2.25 2.25 0 1 1-4.5 0 2.25 2.25 0 0 1 4.5 0Z" />
      </svg>
    ),
  },
  {
    path: "/hr/phong-ban",
    label: "Phòng ban & Vị trí",
    allowedRoles: ["HR"],
    icon: (
      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" d="M2.25 21h19.5m-18-18v18m10.5-18v18m6-13.5V21M6.75 6.75h.75m-.75 3h.75m-.75 3h.75m3-6h.75m-.75 3h.75m-.75 3h.75M6.75 21v-3.375c0-.621.504-1.125 1.125-1.125h2.25c.621 0 1.125.504 1.125 1.125V21M3 3h12m-.75 4.5H21m-3.75 3.75h.008v.008h-.008v-.008Zm0 3h.008v.008h-.008v-.008Zm0 3h.008v.008h-.008v-.008Z" />
      </svg>
    ),
  },
  {
    path: "/hr/ngach-luong",
    label: "Quản lý ngạch lương",
    allowedRoles: ["HR"],
    icon: (
      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" d="M12 6v12m-3-2.818l.879.659c1.171.879 3.07.879 4.242 0 1.172-.879 1.172-2.303 0-3.182C13.536 12.219 12.768 12 12 12c-.725 0-1.45-.22-2.003-.659-1.106-.879-1.106-2.303 0-3.182s2.9-.879 4.006 0l.415.33M21 12a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z" />
      </svg>
    ),
  },
  {
    path: "/hr/chuc-vu",
    label: "Quản lý chức vụ",
    allowedRoles: ["HR"],
    icon: (
      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M20.25 14.15v4.25c0 1.094-.787 2.036-1.872 2.18-2.087.277-4.216.42-6.378.42s-4.291-.143-6.378-.42c-1.085-.144-1.872-1.086-1.872-2.18v-4.25m16.5 0a2.18 2.18 0 0 0 .75-1.661V8.706c0-1.081-.768-2.015-1.837-2.175a48.114 48.114 0 0 0-3.413-.387m4.5 8.006c-.194.165-.42.295-.673.38A23.978 23.978 0 0 1 12 15.75c-2.648 0-5.195-.429-7.577-1.22a2.016 2.016 0 0 1-.673-.38m0 0A2.18 2.18 0 0 1 3 12.489V8.706c0-1.081.768-2.015 1.837-2.175a48.111 48.111 0 0 1 3.413-.387m7.5 0V5.25A2.25 2.25 0 0 0 13.5 3h-3a2.25 2.25 0 0 0-2.25 2.25v.894m7.5 0a48.667 48.667 0 0 0-7.5 0M12 12.75h.008v.008H12v-.008Z" /></svg>
    ),
  },
  {
    path: "/time/lich-lam-viec",
    label: "Lịch làm việc",
    allowedRoles: ["Admin", "HR", "Employee"],
    icon: (
      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 0 1 2.25-2.25h13.5A2.25 2.25 0 0 1 21 7.5v11.25m-18 0A2.25 2.25 0 0 0 5.25 21h13.5A2.25 2.25 0 0 0 21 18.75m-18 0v-7.5A2.25 2.25 0 0 1 5.25 9h13.5A2.25 2.25 0 0 1 21 11.25v7.5m-9-6h.008v.008H12v-.008ZM12 15h.008v.008H12V15Zm0 2.25h.008v.008H12v-.008ZM9.75 15h.008v.008H9.75V15Zm0 2.25h.008v.008H9.75v-.008ZM7.5 15h.008v.008H7.5V15Zm0 2.25h.008v.008H7.5v-.008Zm6.75-4.5h.008v.008h-.008v-.008Zm0 2.25h.008v.008h-.008V15Zm0 2.25h.008v.008h-.008v-.008Zm2.25-4.5h.008v.008H16.5v-.008Zm0 2.25h.008v.008H16.5V15Z" /></svg>
    ),
  },
  {
    path: "/time/cham-cong",
    label: "Chấm công",
    allowedRoles: ["Admin", "HR"],
    icon: (
      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" d="M12 6v6h4.5m4.5 0a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z" />
      </svg>
    ),
  },
  {
    path: "/time/don-nghi",
    label: "Đơn xin nghỉ",
    allowedRoles: ["Admin", "HR"],
    icon: (
      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 14.25v-2.625a3.375 3.375 0 0 0-3.375-3.375h-1.5A1.125 1.125 0 0 1 13.5 7.125v-1.5a3.375 3.375 0 0 0-3.375-3.375H8.25m0 12.75h7.5m-7.5 3H12M10.5 2.25H5.625c-.621 0-1.125.504-1.125 1.125v17.25c0 .621.504 1.125 1.125 1.125h12.75c.621 0 1.125-.504 1.125-1.125V11.25a9 9 0 0 0-9-9Z" />
      </svg>
    ),
  },
  {
    label: "Quản lý đánh giá năng lực",
    allowedRoles: ["Admin", "HR", "Employee"],
    icon: (
      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
        <path strokeLinecap="round" strokeLinejoin="round" d="M11.48 3.499a.562.562 0 0 1 1.04 0l2.125 5.111a.563.563 0 0 0 .475.345l5.518.442c.499.04.701.663.321.988l-4.204 3.602a.563.563 0 0 0-.182.557l1.285 5.385a.562.562 0 0 1-.84.61l-4.725-2.885a.562.562 0 0 0-.586 0L6.982 20.54a.562.562 0 0 1-.84-.61l1.285-5.386a.562.562 0 0 0-.182-.557l-4.204-3.602a.562.562 0 0 1 .321-.988l5.518-.442a.563.563 0 0 0 .475-.345L11.48 3.5Z" />
      </svg>
    ),
    children: [
      {
        path: "/performance/khung-nang-luc",
        label: "Khung năng lực",
        allowedRoles: ["HR"],
      },
      {
        path: "/performance/cau-hinh",
        label: "Cấu hình Mức quy đổi",
        allowedRoles: ["HR"],
      },
      {
        path: "/performance/ky-danh-gia",
        label: "Kỳ Đánh giá",
        allowedRoles: ["Admin", "HR"],
      },
      {
        path: "/performance/tu-danh-gia",
        label: "Tự đánh giá",
        allowedRoles: ["Employee", "Admin", "HR"],
      },
      {
        path: "/performance/duyet-danh-gia",
        label: "Duyệt đánh giá của nhân viên",
        allowedRoles: ["Admin", "HR", "Employee"],
      },
    ],
  },
];

export const Sidebar: React.FC<SidebarProps> = ({ isCollapsed, isOpenMobile, onCloseMobile }) => {
  const { user } = useAuthStore();
  const location = useLocation();
  const userRole = user?.role || "";
  
  const [openMenus, setOpenMenus] = useState<Record<string, boolean>>({});

  useEffect(() => {
    menuItems.forEach(item => {
      if (item.children && item.children.some(child => location.pathname === child.path || location.pathname.startsWith(`${child.path}/`))) {
        setOpenMenus(prev => ({ ...prev, [item.label]: true }));
      }
    });
  }, [location.pathname]);

  const toggleMenu = (label: string) => {
    setOpenMenus(prev => ({ ...prev, [label]: !prev[label] }));
  };

  const filteredMenuItems = menuItems.filter((item) =>
    item.allowedRoles.includes(userRole)
  );

  return (
    <>
      {isOpenMobile && <div className="sidebar-overlay" onClick={onCloseMobile} />}
      <aside className={`sidebar-container ${isCollapsed ? "collapsed" : ""} ${isOpenMobile ? "open" : ""}`}>
        <div className="sidebar-logo-area">
          <div className="sidebar-logo-icon">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor">
              <path fillRule="evenodd" d="M7.5 6a4.5 4.5 0 1 1 9 0 4.5 4.5 0 0 1-9 0ZM3.751 20.105a8.25 8.25 0 0 1 16.498 0 .75.75 0 0 1-.437.695A18.683 18.683 0 0 1 12 22.5c-2.786 0-5.433-.608-7.812-1.7a.75.75 0 0 1-.437-.695Z" clipRule="evenodd" />
            </svg>
          </div>
          <span className="sidebar-logo-text">HRMS Pro</span>
        </div>

        <nav className="sidebar-nav">
          <ul className="sidebar-nav-list">
            {filteredMenuItems.map((item) => {
              const isDropdown = !!item.children;
              const isOpen = openMenus[item.label];
              
              const visibleChildren = item.children?.filter(child => {
                if (!child.allowedRoles.includes(userRole)) return false;
                // Ẩn menu Duyệt đánh giá nếu không phải HR và không có quyền quản lý
                if (child.path === "/performance/duyet-danh-gia") {
                  if (userRole !== "Admin" && !user?.hasDirectReports) return false;
                }
                return true;
              }) || [];
              const hasActiveChild = isDropdown && visibleChildren.some(child => location.pathname === child.path || location.pathname.startsWith(`${child.path}/`));

              return (
                <li key={item.label} className="nav-item-wrapper">
                  {isDropdown ? (
                    <div>
                      <button
                        onClick={() => toggleMenu(item.label)}
                        className={`nav-item nav-item-dropdown ${hasActiveChild ? "active" : ""}`}
                        title={isCollapsed ? item.label : undefined}
                      >
                        <div className="nav-item-content">
                          <span className="nav-icon">{item.icon}</span>
                          <span className="nav-text">{item.label}</span>
                        </div>
                        {!isCollapsed && (
                          <svg
                            xmlns="http://www.w3.org/2000/svg"
                            viewBox="0 0 20 20"
                            fill="currentColor"
                            className={`nav-chevron-icon ${isOpen ? "open" : ""}`}
                          >
                            <path fillRule="evenodd" d="M5.22 8.22a.75.75 0 0 1 1.06 0L10 11.94l3.72-3.72a.75.75 0 1 1 1.06 1.06l-4.25 4.25a.75.75 0 0 1-1.06 0L5.22 9.28a.75.75 0 0 1 0-1.06Z" clipRule="evenodd" />
                          </svg>
                        )}
                      </button>

                      {isOpen && !isCollapsed && visibleChildren.length > 0 && (
                        <div className="submenu-container">
                          <ul className="submenu-list">
                            {visibleChildren.map(child => (
                              <li key={child.path}>
                                <NavLink
                                  to={child.path}
                                  className={({ isActive }) => `submenu-item ${isActive || location.pathname.startsWith(`${child.path}/`) ? "active" : ""}`}
                                >
                                  {child.label}
                                </NavLink>
                              </li>
                            ))}
                          </ul>
                        </div>
                      )}
                    </div>
                  ) : (
                    <NavLink
                      to={item.path!}
                      end={item.path === "/profile"}
                      className={({ isActive }) => `nav-item ${isActive ? "active" : ""}`}
                      title={isCollapsed ? item.label : undefined}
                    >
                      <div className="nav-item-content">
                        <span className="nav-icon">{item.icon}</span>
                        <span className="nav-text">{item.label}</span>
                      </div>
                    </NavLink>
                  )}
                </li>
              );
            })}
          </ul>
        </nav>
      </aside>
    </>
  );
};