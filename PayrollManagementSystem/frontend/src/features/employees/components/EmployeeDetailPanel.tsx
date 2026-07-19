import React, { useState } from 'react';
import { UserProfileDetail } from '@/types/profile.types';
import './EmployeeModals.css';

interface Props {
  employee: UserProfileDetail | null;
  isOpen: boolean;
  onClose: () => void;
}

export const EmployeeDetailPanel: React.FC<Props> = ({ employee, isOpen, onClose }) => {
  const [activeTab, setActiveTab] = useState<'info' | 'relatives' | 'salary'>('info');

  const getInitials = (name: string) => {
    if (!name) return 'U';
    const parts = name.trim().split(' ');
    if (parts.length === 1) return parts[0].charAt(0).toUpperCase();
    return (parts[0].charAt(0) + parts[parts.length - 1].charAt(0)).toUpperCase();
  };

  const formatDate = (dateString: string | null) => {
    if (!dateString) return '—';
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' }).format(date);
  };

  if (!isOpen) return null;

  return (
    <div className="emp-drawer-overlay" onClick={onClose}>
      <div className="emp-drawer" onClick={e => e.stopPropagation()}>
        <div className="emp-drawer-header">
          {employee ? (
            <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
              <div className="emp-avatar" style={{ width: '3.5rem', height: '3.5rem', fontSize: '1.25rem' }}>
                {getInitials(employee.hoTen)}
              </div>
              <div style={{ display: 'flex', flexDirection: 'column' }}>
                <h2 style={{ fontSize: '1.1rem', fontWeight: 700, margin: '0 0 0.25rem 0', color: '#111827' }}>
                  {employee.hoTen}
                </h2>
                <div style={{ display: 'flex', gap: '0.5rem', flexWrap: 'wrap' }}>
                  <span style={{ fontSize: '0.75rem', fontFamily: 'monospace', background: '#f3f4f6', padding: '0.1rem 0.4rem', borderRadius: '4px', border: '1px solid #e5e7eb' }}>
                    {employee.cccd}
                  </span>
                  <span style={{ fontSize: '0.75rem', fontWeight: 600, color: '#6d28d9', background: '#f5f3ff', padding: '0.1rem 0.4rem', borderRadius: '4px', border: '1px solid #ede9fe' }}>
                    {employee.tenChucVu || 'Chưa cập nhật'}
                  </span>
                </div>
              </div>
            </div>
          ) : (
            <span>Đang tải...</span>
          )}
          <button className="emp-drawer-close" onClick={onClose}>&times;</button>
        </div>

        <div className="emp-tabs">
          <button 
            className={`emp-tab ${activeTab === 'info' ? 'active' : ''}`}
            onClick={() => setActiveTab('info')}
          >
            Hồ sơ cá nhân
          </button>
          <button 
            className={`emp-tab ${activeTab === 'relatives' ? 'active' : ''}`}
            onClick={() => setActiveTab('relatives')}
          >
            Thân nhân
          </button>
          <button 
            className={`emp-tab ${activeTab === 'salary' ? 'active' : ''}`}
            onClick={() => setActiveTab('salary')}
          >
            Lương & HĐ
          </button>
        </div>

        <div className="emp-drawer-body custom-scrollbar">
          {employee && activeTab === 'info' && (
            <>
              <div className="emp-detail-section">
                <h3 className="emp-detail-section-title">Liên hệ</h3>
                <div className="emp-detail-card">
                  <div className="emp-detail-row">
                    <span className="emp-detail-label">Email</span>
                    <span className="emp-detail-value">{employee.email || '—'}</span>
                  </div>
                  <div className="emp-detail-row">
                    <span className="emp-detail-label">Số điện thoại</span>
                    <span className="emp-detail-value">{employee.sdt || '—'}</span>
                  </div>
                  <div className="emp-detail-row" style={{ alignItems: 'flex-start' }}>
                    <span className="emp-detail-label">Địa chỉ</span>
                    <span className="emp-detail-value" style={{ textAlign: 'right' }}>{employee.diaChi || '—'}</span>
                  </div>
                </div>
              </div>

              <div className="emp-detail-section">
                <h3 className="emp-detail-section-title">Lý lịch</h3>
                <div className="emp-detail-card">
                  <div className="emp-detail-row">
                    <span className="emp-detail-label">Ngày sinh</span>
                    <span className="emp-detail-value">{formatDate(employee.ngaySinh)}</span>
                  </div>
                  <div className="emp-detail-row">
                    <span className="emp-detail-label">Giới tính</span>
                    <span className="emp-detail-value">
                      {employee.gioiTinh === true ? 'Nam' : employee.gioiTinh === false ? 'Nữ' : '—'}
                    </span>
                  </div>
                  <div className="emp-detail-row">
                    <span className="emp-detail-label">Dân tộc</span>
                    <span className="emp-detail-value">{employee.danToc || '—'}</span>
                  </div>
                </div>
              </div>

              <div className="emp-detail-section">
                <h3 className="emp-detail-section-title">Công tác & BHXH</h3>
                <div className="emp-detail-card">
                  <div className="emp-detail-row">
                    <span className="emp-detail-label">Phòng ban</span>
                    <span className="emp-detail-value">{employee.tenPhongBan || '—'}</span>
                  </div>
                  <div className="emp-detail-row">
                    <span className="emp-detail-label">Chuyên ngành</span>
                    <span className="emp-detail-value">{employee.chuyenNganh || '—'}</span>
                  </div>
                  <div className="emp-detail-row">
                    <span className="emp-detail-label">Số BHXH</span>
                    <span className="emp-detail-value mono">{employee.soBhxh || '—'}</span>
                  </div>
                </div>
              </div>

              <div className="emp-detail-section" style={{ paddingBottom: '2rem' }}>
                <h3 className="emp-detail-section-title">Tài chính & Thuế</h3>
                <div className="emp-detail-card">
                  <div className="emp-detail-row">
                    <span className="emp-detail-label">Số tài khoản</span>
                    <span className="emp-detail-value mono">{employee.soTaiKhoan || '—'}</span>
                  </div>
                  <div className="emp-detail-row">
                    <span className="emp-detail-label">Ngân hàng</span>
                    <span className="emp-detail-value">{employee.tenNganHang || '—'}</span>
                  </div>
                  <div className="emp-detail-row">
                    <span className="emp-detail-label">Mã số thuế</span>
                    <span className="emp-detail-value mono">{employee.maSoThue || '—'}</span>
                  </div>
                </div>
              </div>
            </>
          )}

          {employee && activeTab === 'relatives' && (
            <div className="emp-detail-section" style={{ paddingBottom: '2rem' }}>
              {(employee.thanNhans?.length ?? 0) > 0 ? (
                <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
                  {employee.thanNhans.map((tn, index) => (
                    <div key={index} className="emp-detail-card" style={{ padding: '1rem' }}>
                      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '0.5rem' }}>
                        <h4 style={{ margin: 0, fontWeight: 700, fontSize: '1rem', color: '#111827' }}>{tn.tenTn}</h4>
                        <span style={{ background: '#f5f3ff', color: '#6d28d9', fontSize: '0.75rem', fontWeight: 600, padding: '0.2rem 0.6rem', borderRadius: '99px', border: '1px solid #ede9fe' }}>
                          {tn.moiQuanHe || 'Không rõ'}
                        </span>
                      </div>
                      <div style={{ display: 'flex', justifyContent: 'space-between', paddingTop: '0.5rem', borderTop: '1px solid #f3f4f6' }}>
                        <span style={{ fontSize: '0.8rem', color: '#6b7280' }}>Năm sinh</span>
                        <span style={{ fontSize: '0.85rem', fontWeight: 600 }}>{formatDate(tn.ngaySinh) || '—'}</span>
                      </div>
                    </div>
                  ))}
                </div>
              ) : (
                <div style={{ padding: '3rem 1rem', textAlign: 'center', color: '#9ca3af' }}>
                  <p>Chưa có thông tin thân nhân</p>
                </div>
              )}
            </div>
          )}

          {employee && activeTab === 'salary' && (
            <>
              <div className="emp-detail-section">
                <h3 className="emp-detail-section-title">Lương 3P</h3>
                <div className="emp-detail-card">
                  <div className="emp-detail-row">
                    <span className="emp-detail-label">Lương P1 (Vị trí)</span>
                    <span className="emp-detail-value" style={{ color: '#6d28d9' }}>
                      {employee.luongP1 ? new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(employee.luongP1) : 'Chưa cập nhật'}
                    </span>
                  </div>
                  <div className="emp-detail-row">
                    <span className="emp-detail-label">Hệ số P2 (Năng lực)</span>
                    <span className="emp-detail-value" style={{ color: '#d97706' }}>
                      {employee.heSoP2 ? `${employee.heSoP2}` : '1.00'}
                    </span>
                  </div>
                </div>
              </div>

              <div className="emp-detail-section" style={{ paddingBottom: '2rem' }}>
                <h3 className="emp-detail-section-title">Hợp đồng hiện tại</h3>
                <div className="emp-detail-card">
                  <div className="emp-detail-row">
                    <span className="emp-detail-label">Số HĐ</span>
                    <span className="emp-detail-value mono">{employee.soHopDong || '—'}</span>
                  </div>
                  <div className="emp-detail-row">
                    <span className="emp-detail-label">Loại HĐ</span>
                    <span className="emp-detail-value">{employee.loaiHopDong || '—'}</span>
                  </div>
                  <div className="emp-detail-row">
                    <span className="emp-detail-label">Ngày bắt đầu</span>
                    <span className="emp-detail-value">{formatDate(employee.ngayBatDauHopDong ?? null)}</span>
                  </div>
                </div>
              </div>
            </>
          )}
        </div>
      </div>
    </div>
  );
};