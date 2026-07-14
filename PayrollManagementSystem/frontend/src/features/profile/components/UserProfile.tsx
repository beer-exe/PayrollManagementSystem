import React, { useState, useEffect } from 'react';
import { UserProfileDetail } from '@/types/profile.types';
import { profileApi } from '../api/profileApi';
import { motion, AnimatePresence } from 'framer-motion';
import './UserProfile.css';

type TabType = 'personal' | 'contract' | 'finance' | 'dependents' | 'history';

export const UserProfile: React.FC = () => {
  const [profile, setProfile] = useState<UserProfileDetail | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const [activeTab, setActiveTab] = useState<TabType>('personal');

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        setIsLoading(true);
        setError(null);
        
        const response = await profileApi.getMyProfile();
        
        if (response.succeeded && response.data) {
          setProfile(response.data);
        } else {
          setError(response.message || 'Không thể tải thông tin hồ sơ.');
        }
      } catch (error) {
        const err = error as import('axios').AxiosError<{Message?: string}>;
        const errorMessage = err?.response?.data?.Message || err?.message || 'Đã xảy ra lỗi khi kết nối với máy chủ.';
        setError(errorMessage);
      } finally {
        setIsLoading(false);
      }
    };

    fetchProfile();
  }, []);

  const getInitials = (name: string) => {
    if (!name) return 'U';
    const parts = name.trim().split(' ');
    if (parts.length === 1) return parts[0].charAt(0).toUpperCase();
    return (parts[0].charAt(0) + parts[parts.length - 1].charAt(0)).toUpperCase();
  };

  const formatDate = (dateString: string | null | undefined) => {
    if (!dateString) return 'Chưa cập nhật';
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('vi-VN').format(date);
  };

  const formatCurrency = (amount: number | null | undefined) => {
    if (amount === null || amount === undefined) return 'Chưa cập nhật';
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
  };

  const getDecisionStatus = (history: any, idx: number, allHistories: any[]) => {
    if (history.trangThai === 'HUY_BO') {
      return { text: history.tenTrangThai || 'Hủy bỏ', color: 'text-red-600 dark:text-red-400', dot: 'bg-red-400' };
    }

    if (history.trangThai === 'HET_HAN') {
      return { text: history.tenTrangThai || 'Hết hạn', color: 'text-gray-500 dark:text-gray-400', dot: 'bg-gray-400' };
    }

    const todayStr = new Date().toLocaleDateString('en-CA'); 
    
    if (history.trangThai === 'HIEU_LUC') {
        if (history.ngayHieuLuc > todayStr) {
            return { text: 'Chờ áp dụng', color: 'text-yellow-600 dark:text-yellow-400', dot: 'bg-yellow-500 shadow-[0_0_0_3px_rgba(234,179,8,0.2)]' };
        }
        
        const currentActiveIdx = allHistories.findIndex((h: any) => h.trangThai === 'HIEU_LUC' && h.ngayHieuLuc <= todayStr);
        if (idx === currentActiveIdx) {
            return { text: 'Đang áp dụng', color: 'text-green-600 dark:text-green-400', dot: 'bg-green-500 shadow-[0_0_0_3px_rgba(34,197,94,0.2)]' };
        }
        
        return { text: 'Đã qua', color: 'text-gray-500 dark:text-gray-400', dot: 'bg-gray-400' };
    }

    return { text: history.tenTrangThai || history.trangThai, color: 'text-gray-500 dark:text-gray-400', dot: 'bg-gray-400' };
  };

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-[calc(100vh-8rem)]">
        <div className="flex flex-col items-center gap-3">
          <div className="animate-spin rounded-full h-10 w-10 border-b-2 border-violet-600"></div>
          <p className="text-gray-500 font-medium">Đang tải hồ sơ...</p>
        </div>
      </div>
    );
  }

  if (error || !profile) {
    return (
      <div className="flex justify-center items-center h-[calc(100vh-8rem)]">
        <div className="bg-red-50 dark:bg-red-900/10 border border-red-200 dark:border-red-800 text-red-600 dark:text-red-400 p-6 rounded-xl max-w-md text-center shadow-sm">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-12 h-12 mx-auto mb-3 opacity-80">
            <path strokeLinecap="round" strokeLinejoin="round" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
          </svg>
          <h3 className="text-lg font-bold mb-2">Lấy dữ liệu thất bại</h3>
          <p className="text-sm mb-4">{error || 'Không có dữ liệu'}</p>
          <button 
            onClick={() => window.location.reload()} 
            className="px-4 py-2 bg-red-100 hover:bg-red-200 dark:bg-red-900/30 dark:hover:bg-red-900/50 rounded-md transition-colors font-medium text-sm"
          >
            Thử lại
          </button>
        </div>
      </div>
    );
  }

  const tabs = [
    { 
      id: 'personal', 
      label: 'Thông tin cá nhân', 
      icon: <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-5 h-5"><path strokeLinecap="round" strokeLinejoin="round" d="M15.75 6a3.75 3.75 0 1 1-7.5 0 3.75 3.75 0 0 1 7.5 0ZM4.501 20.118a7.5 7.5 0 0 1 14.998 0A17.933 17.933 0 0 1 12 21.75c-2.676 0-5.216-.584-7.499-1.632Z" /></svg>
    },
    { 
      id: 'contract', 
      label: 'Hợp đồng & Bảo hiểm', 
      icon: <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-5 h-5"><path strokeLinecap="round" strokeLinejoin="round" d="M19.5 14.25v-2.625a3.375 3.375 0 0 0-3.375-3.375h-1.5A1.125 1.125 0 0 1 13.5 7.125v-1.5a3.375 3.375 0 0 0-3.375-3.375H8.25m0 12.75h7.5m-7.5 3H12M10.5 2.25H5.625c-.621 0-1.125.504-1.125 1.125v17.25c0 .621.504 1.125 1.125 1.125h12.75c.621 0 1.125-.504 1.125-1.125V11.25a9 9 0 0 0-9-9Z" /></svg>
    },
    { 
      id: 'finance', 
      label: 'Tài chính & Lương', 
      icon: <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-5 h-5"><path strokeLinecap="round" strokeLinejoin="round" d="M2.25 18.75a60.07 60.07 0 0 1 15.797 2.101c.727.198 1.453-.342 1.453-1.096V4.22c0-.756-.728-1.294-1.453-1.096A60.864 60.864 0 0 1 2.25 5.25m0 13.5L19.5 5.25m-17.25 13.5L19.5 5.25" /></svg>
    },
    { 
      id: 'dependents', 
      label: 'Gia đình & NPT', 
      icon: <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-5 h-5"><path strokeLinecap="round" strokeLinejoin="round" d="M18 18.72a9.094 9.094 0 0 0 3.741-.479 3 3 0 0 0-4.682-2.72m.94 3.198.001.031c0 .225-.012.447-.037.666A11.944 11.944 0 0 1 12 21.75c-2.676 0-5.216-.584-7.499-1.632l-.046-.022a4.116 4.116 0 0 0-.037-.666 3 3 0 0 1 4.682-2.72 9.09 9.09 0 0 1 3.741.479m.94 3.198A8.995 8.995 0 0 1 12 22.5c-2.786 0-5.433-.608-7.812-1.7a.75.75 0 0 1-.437-.695A18.683 18.683 0 0 1 12 22.5c-2.786 0-5.433-.608-7.812-1.7a.75.75 0 0 1-.437-.695Z" /></svg>
    },
    { 
      id: 'history', 
      label: 'Lịch sử công tác', 
      icon: <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-5 h-5"><path strokeLinecap="round" strokeLinejoin="round" d="M12 6v6h4.5m4.5 0a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z" /></svg>
    },
  ];

  const renderTabContent = () => {
    switch (activeTab) {
      case 'personal':
        return (
          <motion.div
            key="personal"
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -10 }}
            transition={{ duration: 0.2 }}
          >
            <div className="info-section">
              <h3 className="info-section-title">Danh tính</h3>
              <div className="info-grid">
                <div className="info-block">
                  <span className="info-label">Số CCCD</span>
                  <span className="info-value">{profile.cccd}</span>
                </div>
                <div className="info-block">
                  <span className="info-label">Ngày sinh</span>
                  <span className="info-value">{formatDate(profile.ngaySinh)}</span>
                </div>
                <div className="info-block">
                  <span className="info-label">Giới tính</span>
                  <span className="info-value">{profile.gioiTinh === true ? 'Nam' : profile.gioiTinh === false ? 'Nữ' : 'Chưa cập nhật'}</span>
                </div>
                <div className="info-block">
                  <span className="info-label">Dân tộc</span>
                  <span className="info-value">{profile.danToc || 'Chưa cập nhật'}</span>
                </div>
              </div>
            </div>

            <div className="info-section mt-8">
              <h3 className="info-section-title">Liên hệ</h3>
              <div className="info-grid">
                <div className="info-block">
                  <span className="info-label">Số điện thoại</span>
                  <span className="info-value">{profile.sdt || 'Chưa cập nhật'}</span>
                </div>
                <div className="info-block">
                  <span className="info-label">Email</span>
                  <span className="info-value">{profile.email || 'Chưa cập nhật'}</span>
                </div>
                <div className="info-block sm:col-span-2 lg:col-span-3">
                  <span className="info-label">Địa chỉ</span>
                  <span className="info-value">{profile.diaChi || 'Chưa cập nhật'}</span>
                </div>
              </div>
            </div>
            
            <div className="info-section mt-8">
              <h3 className="info-section-title">Chuyên môn</h3>
              <div className="info-grid">
                <div className="info-block sm:col-span-2">
                  <span className="info-label">Chuyên ngành đào tạo</span>
                  <span className="info-value">{profile.chuyenNganh || 'Chưa cập nhật'}</span>
                </div>
              </div>
            </div>
          </motion.div>
        );
      
      case 'contract':
        return (
          <motion.div
            key="contract"
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -10 }}
            transition={{ duration: 0.2 }}
          >
            <div className="info-section">
              <h3 className="info-section-title">Thông tin Hợp đồng</h3>
              <div className="info-grid">
                <div className="info-block">
                  <span className="info-label">Số hợp đồng</span>
                  <span className="info-value">{profile.soHopDong || 'Chưa cập nhật'}</span>
                </div>
                <div className="info-block">
                  <span className="info-label">Loại hợp đồng</span>
                  <span className="info-value">{profile.loaiHopDong || 'Chưa cập nhật'}</span>
                </div>
                <div className="info-block">
                  <span className="info-label">Ngày bắt đầu</span>
                  <span className="info-value">{formatDate(profile.ngayBatDauHopDong)}</span>
                </div>
                <div className="info-block">
                  <span className="info-label">Ngày vào làm</span>
                  <span className="info-value">{formatDate(profile.ngayVaoLam)}</span>
                </div>
              </div>
            </div>

            <div className="info-section mt-8">
              <h3 className="info-section-title">Bảo hiểm</h3>
              <div className="info-grid">
                <div className="info-block">
                  <span className="info-label">Mã số BHXH</span>
                  <span className="info-value font-mono">{profile.soBhxh || 'Chưa cập nhật'}</span>
                </div>
                <div className="info-block">
                  <span className="info-label">Mã thẻ BHYT</span>
                  <span className="info-value font-mono">{profile.soBhyt || 'Chưa cập nhật'}</span>
                </div>
              </div>
            </div>
          </motion.div>
        );

      case 'finance':
        return (
          <motion.div
            key="finance"
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -10 }}
            transition={{ duration: 0.2 }}
          >
            <div className="info-section">
              <h3 className="info-section-title">Thông tin Lương</h3>
              <div className="info-grid">
                <div className="info-block bg-violet-50/50 dark:bg-violet-900/10 border-violet-100 dark:border-violet-800">
                  <span className="info-label text-violet-600 dark:text-violet-400">Lương cơ bản (P1)</span>
                  <span className="info-value text-xl font-bold text-violet-700 dark:text-violet-300">
                    {formatCurrency(profile.luongP1)}
                  </span>
                </div>
                <div className="info-block">
                  <span className="info-label">Hệ số năng lực (P2)</span>
                  <span className="info-value text-xl font-bold">{profile.heSoP2 !== null ? profile.heSoP2 : 'Chưa thiết lập'}</span>
                </div>
              </div>
            </div>

            <div className="info-section mt-8">
              <h3 className="info-section-title">Ngân hàng & Thuế</h3>
              <div className="info-grid">
                <div className="info-block">
                  <span className="info-label">Ngân hàng</span>
                  <span className="info-value">{profile.tenNganHang || 'Chưa cập nhật'}</span>
                </div>
                <div className="info-block">
                  <span className="info-label">Số tài khoản</span>
                  <span className="info-value font-mono">{profile.soTaiKhoan || 'Chưa cập nhật'}</span>
                </div>
                <div className="info-block">
                  <span className="info-label">Mã số thuế</span>
                  <span className="info-value font-mono">{profile.maSoThue || 'Chưa cập nhật'}</span>
                </div>
              </div>
            </div>
          </motion.div>
        );

      case 'dependents':
        return (
          <motion.div
            key="dependents"
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -10 }}
            transition={{ duration: 0.2 }}
          >
            <div className="info-section">
              <div className="flex justify-between items-center mb-6">
                <h3 className="info-section-title mb-0">Người phụ thuộc</h3>
                <span className="px-3 py-1 bg-violet-100 dark:bg-violet-900/40 text-violet-700 dark:text-violet-300 rounded-full text-xs font-bold">
                  {profile.thanNhans?.length || 0} người
                </span>
              </div>
              
              {(!profile.thanNhans || profile.thanNhans.length === 0) ? (
                <div className="text-center p-12 border-2 border-dashed border-gray-200 dark:border-gray-700 rounded-2xl bg-gray-50/50 dark:bg-gray-800/30">
                  <p className="text-gray-500 dark:text-gray-400 font-medium">Chưa có thông tin người phụ thuộc</p>
                </div>
              ) : (
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  {profile.thanNhans.map((tn, idx) => (
                    <div key={idx} className="dependent-card">
                      <div className="flex items-start justify-between mb-3">
                        <div>
                          <h4 className="font-bold text-gray-900 dark:text-white text-base">{tn.tenTn}</h4>
                          <span className="inline-block mt-1 px-2 py-0.5 bg-gray-100 dark:bg-gray-700 text-gray-600 dark:text-gray-300 rounded text-xs font-medium">
                            {tn.moiQuanHe || 'Chưa rõ'}
                          </span>
                        </div>
                      </div>
                      <div className="space-y-2 mt-4 text-sm text-gray-600 dark:text-gray-400">
                        <div className="flex justify-between border-b border-gray-100 dark:border-gray-700 pb-2">
                          <span>Mã định danh:</span>
                          <span className="font-medium text-gray-900 dark:text-gray-200">{tn.maDinhDanh || '---'}</span>
                        </div>
                        <div className="flex justify-between pt-1">
                          <span>Ngày sinh:</span>
                          <span className="font-medium text-gray-900 dark:text-gray-200">{formatDate(tn.ngaySinh)}</span>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </motion.div>
        );

      case 'history':
        return (
          <motion.div
            key="history"
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -10 }}
            transition={{ duration: 0.2 }}
          >
            <div className="info-section">
              <h3 className="info-section-title mb-6">Lịch sử Quyết định & Công tác</h3>
              
              {(!profile.lichSuCongTac || profile.lichSuCongTac.length === 0) ? (
                <div className="text-center p-12 border-2 border-dashed border-gray-200 dark:border-gray-700 rounded-2xl bg-gray-50/50 dark:bg-gray-800/30">
                  <p className="text-gray-500 dark:text-gray-400 font-medium">Chưa có dữ liệu lịch sử công tác</p>
                </div>
              ) : (
                <div className="relative border-l-2 border-violet-200 dark:border-violet-900/50 ml-3 md:ml-4 space-y-8">
                  {profile.lichSuCongTac.map((history, idx) => {
                    const status = getDecisionStatus(history, idx, profile.lichSuCongTac!);
                    return (
                    <div key={idx} className="relative pl-6 md:pl-8">
                      {/* Timeline Dot */}
                      <span className={`absolute -left-[9px] top-1.5 w-4 h-4 rounded-full border-2 border-white dark:border-gray-800 ${status.dot}`}></span>
                      
                      <div className="bg-white dark:bg-gray-800 p-5 rounded-xl border border-gray-100 dark:border-gray-700 shadow-sm hover:shadow-md transition-shadow">
                        <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center mb-3 gap-2">
                          <h4 className="font-bold text-gray-900 dark:text-white text-base md:text-lg">
                            {history.loaiQuyetDinh}
                          </h4>
                          <span className="px-3 py-1 bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 rounded-md text-xs font-semibold whitespace-nowrap">
                            {formatDate(history.ngayHieuLuc)}
                          </span>
                        </div>
                        
                        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 mt-4">
                          <div className="flex flex-col">
                            <span className="text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase mb-1">Số QĐ</span>
                            <span className="text-sm font-medium text-gray-900 dark:text-gray-200">{history.soQuyetDinh}</span>
                          </div>
                          
                          {history.tenChucVuMoi && (
                            <div className="flex flex-col">
                              <span className="text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase mb-1">Chức vụ</span>
                              <span className="text-sm font-medium text-violet-700 dark:text-violet-400">{history.tenChucVuMoi}</span>
                            </div>
                          )}
                          
                          {history.luongP1Moi && (
                            <div className="flex flex-col">
                              <span className="text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase mb-1">Lương cơ bản (P1)</span>
                              <span className="text-sm font-bold text-gray-900 dark:text-white">{formatCurrency(history.luongP1Moi)}</span>
                            </div>
                          )}
                          
                          <div className="flex flex-col">
                            <span className="text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase mb-1">Trạng thái</span>
                            <span className={`text-sm font-medium ${status.color}`}>
                              {status.text}
                            </span>
                          </div>
                        </div>
                      </div>
                    </div>
                  )})}
                </div>
              )}
            </div>
          </motion.div>
        );
    }
  };

  return (
    <div className="profile-layout">
      {/* Left Sidebar Pane */}
      <div className="profile-sidebar">
        <div className="profile-avatar-card">
          <div className="profile-avatar-header">
            <div className="profile-avatar-large">
              {getInitials(profile.hoTen)}
            </div>
          </div>
          <div className="profile-avatar-info">
            <h1 className="profile-name">{profile.hoTen}</h1>
            <p className="profile-role">
              {profile.tenChucVu || 'Chưa phân bổ chức vụ'} • {profile.tenPhongBan || 'Chưa thuộc phòng ban'}
            </p>
            <span className="profile-badge">
              {profile.trangThai === 'DANG_LAM_VIEC' ? (
                <><span className="w-1.5 h-1.5 rounded-full bg-green-500 mr-1.5"></span>{profile.tenTrangThai || 'Đang làm việc'}</>
              ) : (
                <><span className="w-1.5 h-1.5 rounded-full bg-gray-500 mr-1.5"></span>{profile.tenTrangThai || 'Đã nghỉ việc'}</>
              )}
            </span>
          </div>
        </div>

        <div className="profile-tabs-container custom-scrollbar">
          <div className="flex flex-col gap-1 p-2">
            {tabs.map((tab) => (
              <button
                key={tab.id}
                onClick={() => setActiveTab(tab.id as TabType)}
                className={`tab-item ${activeTab === tab.id ? 'tab-item-active' : ''}`}
              >
                <span className="tab-icon">{tab.icon}</span>
                {tab.label}
              </button>
            ))}
          </div>
        </div>
      </div>

      {/* Right Content Pane */}
      <div className="profile-content">
        <div className="profile-content-header">
          {tabs.find(t => t.id === activeTab)?.icon}
          <h2 className="profile-content-title">
            {tabs.find(t => t.id === activeTab)?.label}
          </h2>
        </div>
        
        <div className="profile-content-body custom-scrollbar relative">
          <AnimatePresence mode="wait">
            {renderTabContent()}
          </AnimatePresence>
        </div>
      </div>
    </div>
  );
};