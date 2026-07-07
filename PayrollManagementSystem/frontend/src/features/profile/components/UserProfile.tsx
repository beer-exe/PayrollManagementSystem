import React, { useState, useEffect } from 'react';
import { UserProfileDetail } from '@/types/profile.types';
import { profileApi } from '../api/profileApi';
import './UserProfile.css';

export const UserProfile: React.FC = () => {
  const [profile, setProfile] = useState<UserProfileDetail | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

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
      } catch (error) { const err = error as import('axios').AxiosError<{Message?: string}>;
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

  const formatDate = (dateString: string | null) => {
    if (!dateString) return 'Chưa cập nhật';
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('vi-VN').format(date);
  };

  if (isLoading) {
    return (
      <div className="profile-container flex justify-center items-center min-h-[400px]">
        <div className="flex flex-col items-center gap-3">
          <div className="animate-spin rounded-full h-10 w-10 border-b-2 border-violet-600"></div>
          <p className="text-gray-500 font-medium">Đang tải hồ sơ...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="profile-container flex justify-center items-center min-h-[400px]">
        <div className="bg-red-50 dark:bg-red-900/10 border border-red-200 dark:border-red-800 text-red-600 dark:text-red-400 p-6 rounded-xl max-w-md text-center shadow-sm">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-12 h-12 mx-auto mb-3 opacity-80">
            <path strokeLinecap="round" strokeLinejoin="round" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
          </svg>
          <h3 className="text-lg font-bold mb-2">Lấy dữ liệu thất bại</h3>
          <p className="text-sm mb-4">{error}</p>
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

  if (!profile) return null;

  return (
    <div className="profile-container">
      {/* Header Section */}
      <div className="profile-header-card">
        <div className="profile-avatar-large">
          {getInitials(profile.hoTen)}
        </div>
        <div className="flex-1 flex flex-col sm:flex-row justify-between items-center sm:items-start w-full">
          <div className="flex flex-col items-center sm:items-start text-center sm:text-left">
            <h1 className="profile-name">{profile.hoTen}</h1>
            <p className="text-violet-600 dark:text-violet-400 font-medium mb-1">
              {profile.tenChucVu || 'Chưa phân bổ chức vụ'} • {profile.tenPhongBan || 'Chưa thuộc phòng ban'}
            </p>
            <p className="text-gray-500 text-sm mb-3">{profile.email}</p>
            <span className="profile-badge">
              {profile.trangThai === 'DANG_LAM_VIEC' ? (
                <>
                  <span className="w-1.5 h-1.5 rounded-full bg-green-500 mr-1.5"></span>
                  {profile.tenTrangThai || 'Đang làm việc'}
                </>
              ) : (
                <>
                  <span className="w-1.5 h-1.5 rounded-full bg-gray-500 mr-1.5"></span>
                  {profile.tenTrangThai || 'Đã nghỉ việc'}
                </>
              )}
            </span>
          </div>
          <button className="btn-outline flex items-center gap-2 mt-4 sm:mt-0">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-4 h-4">
              <path strokeLinecap="round" strokeLinejoin="round" d="m16.862 4.487 1.687-1.688a1.875 1.875 0 1 1 2.652 2.652L6.832 19.82a4.5 4.5 0 0 1-1.897 1.13l-2.685.8.8-2.685a4.5 4.5 0 0 1 1.13-1.897L16.863 4.487Zm0 0L19.5 7.125" />
            </svg>
            Yêu cầu cập nhật
          </button>
        </div>
      </div>

      {/* Info Cards Grid */}
      <div className="profile-grid">
        {/* Cột 1: Thông tin cá nhân */}
        <div className="profile-card lg:col-span-2">
          <div className="profile-card-header">
            <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-5 h-5 text-violet-500">
              <path strokeLinecap="round" strokeLinejoin="round" d="M15 9h3.75M15 12h3.75M15 15h3.75M4.5 19.5h15a2.25 2.25 0 0 0 2.25-2.25V6.75A2.25 2.25 0 0 0 19.5 4.5h-15a2.25 2.25 0 0 0-2.25 2.25v10.5A2.25 2.25 0 0 0 4.5 19.5Zm6-10.125a1.875 1.875 0 1 1-3.75 0 1.875 1.875 0 0 1 3.75 0Zm1.294 6.336a6.721 6.721 0 0 1-3.17.789 6.721 6.721 0 0 1-3.168-.789 3.376 3.376 0 0 1 6.338 0Z" />
            </svg>
            <h2 className="profile-card-title">Thông tin cá nhân</h2>
          </div>
          <div className="profile-card-body grid grid-cols-1 sm:grid-cols-2 gap-y-4 gap-x-6">
            <div className="info-group">
              <span className="info-label">Số CCCD</span>
              <span className="info-value">{profile.cccd}</span>
            </div>
            <div className="info-group">
              <span className="info-label">Ngày sinh</span>
              <span className="info-value">{formatDate(profile.ngaySinh)}</span>
            </div>
            <div className="info-group">
              <span className="info-label">Giới tính</span>
              <span className="info-value">
                {profile.gioiTinh === true ? 'Nam' : profile.gioiTinh === false ? 'Nữ' : 'Chưa cập nhật'}
              </span>
            </div>
            <div className="info-group">
              <span className="info-label">Dân tộc</span>
              <span className="info-value">{profile.danToc || 'Chưa cập nhật'}</span>
            </div>
            <div className="info-group">
              <span className="info-label">Số điện thoại</span>
              <span className="info-value">{profile.sdt || 'Chưa cập nhật'}</span>
            </div>
            <div className="info-group sm:col-span-2">
              <span className="info-label">Địa chỉ liên hệ</span>
              <span className="info-value">{profile.diaChi || 'Chưa cập nhật'}</span>
            </div>
          </div>
        </div>

        {/* Cột 2: Thông tin công việc & Bảo hiểm */}
        <div className="space-y-6">
          <div className="profile-card">
            <div className="profile-card-header">
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-5 h-5 text-violet-500">
                <path strokeLinecap="round" strokeLinejoin="round" d="M20.25 14.15v4.25c0 1.094-.787 2.036-1.872 2.18-2.087.277-4.216.42-6.378.42s-4.291-.143-6.378-.42c-1.085-.144-1.872-1.086-1.872-2.18v-4.25m16.5 0a2.18 2.18 0 0 0 .75-1.661V8.706c0-1.081-.768-2.015-1.837-2.175a48.114 48.114 0 0 0-3.413-.387m4.5 8.006c-.194.165-.42.295-.673.38A23.978 23.978 0 0 1 12 15.75c-2.648 0-5.195-.429-7.577-1.22a2.016 2.016 0 0 1-.673-.38m0 0A2.18 2.18 0 0 1 3 12.489V8.706c0-1.081.768-2.015 1.837-2.175a48.111 48.111 0 0 1 3.413-.387m7.5 0V5.25A2.25 2.25 0 0 0 13.5 3h-3a2.25 2.25 0 0 0-2.25 2.25v.894m7.5 0a48.667 48.667 0 0 0-7.5 0M12 12.75h.008v.008H12v-.008Z" />
              </svg>
              <h2 className="profile-card-title">Hợp đồng & Chuyên môn</h2>
            </div>
            <div className="profile-card-body">
              <div className="info-group mb-4">
                <span className="info-label">Ngày vào làm</span>
                <span className="info-value">{formatDate(profile.ngayVaoLam)}</span>
              </div>
              <div className="info-group">
                <span className="info-label">Chuyên ngành đào tạo</span>
                <span className="info-value">{profile.chuyenNganh || 'Chưa cập nhật'}</span>
              </div>
            </div>
          </div>

          <div className="profile-card">
            <div className="profile-card-header">
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className="w-5 h-5 text-violet-500">
                <path strokeLinecap="round" strokeLinejoin="round" d="M9 12.75 11.25 15 15 9.75m-3-7.036A11.959 11.959 0 0 1 3.598 6 11.99 11.99 0 0 0 3 9.749c0 5.592 3.824 10.29 9 11.623 5.176-1.332 9-6.03 9-11.622 0-1.31-.21-2.571-.598-3.751h-.152c-3.196 0-6.1-1.248-8.25-3.285Z" />
              </svg>
              <h2 className="profile-card-title">Bảo hiểm xã hội</h2>
            </div>
            <div className="profile-card-body">
              <div className="info-group mb-4">
                <span className="info-label">Mã số BHXH</span>
                <span className="info-value font-mono">{profile.soBhxh || 'Chưa cập nhật'}</span>
              </div>
              <div className="info-group">
                <span className="info-label">Mã thẻ BHYT</span>
                <span className="info-value font-mono">{profile.soBhyt || 'Chưa cập nhật'}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};