import React, { useState, useEffect, useRef } from 'react';
import { employeeApi } from '../../employees/api/employeeApi';
import type { UserProfileDetail } from '@/types/profile.types';
import type { CreateDonNghiRequest } from '../types/donNghi.types';
import { LOAI_NGHI_OPTIONS } from '../types/donNghi.types';

interface Props {
  onClose: () => void;
  onCreate: (data: CreateDonNghiRequest) => Promise<boolean>;
}

export const DonNghiFormModal: React.FC<Props> = ({ onClose, onCreate }) => {
  const [employees, setEmployees] = useState<UserProfileDetail[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedCccd, setSelectedCccd] = useState('');
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  const [form, setForm] = useState({
    loaiNghi: 'NGHI_PHEP_NAM',
    ngayBatDau: '',
    ngayKetThuc: '',
    soNgayNghi: 1,
    lyDo: '',
    taiLieuDinhKem: '',
  });
  const [submitting, setSubmitting] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});

  // Load employees
  useEffect(() => {
    employeeApi.getEmployees({ PageNumber: 1, PageSize: 500 })
      .then(res => setEmployees((res as { data: UserProfileDetail[] }).data ?? []))
      .catch(console.error);

    const handleClickOutside = (e: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(e.target as Node)) {
        setIsDropdownOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const filteredEmployees = employees.filter(emp =>
    emp.hoTen?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    emp.cccd?.includes(searchTerm)
  );

  const handleSelectEmployee = (emp: UserProfileDetail) => {
    setSelectedCccd(emp.cccd ?? '');
    setSearchTerm(`${emp.hoTen} - ${emp.cccd}`);
    setIsDropdownOpen(false);
    setErrors(e => ({ ...e, cccd: '' }));
  };

  // Auto-calc SoNgayNghi when dates change
  useEffect(() => {
    if (form.ngayBatDau && form.ngayKetThuc) {
      const start = new Date(form.ngayBatDau);
      const end = new Date(form.ngayKetThuc);
      if (end >= start) {
        const diff = Math.round((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24)) + 1;
        setForm(f => ({ ...f, soNgayNghi: diff }));
      }
    }
  }, [form.ngayBatDau, form.ngayKetThuc]);

  const validate = (): boolean => {
    const errs: Record<string, string> = {};
    if (!selectedCccd) errs.cccd = 'Vui lòng chọn nhân viên.';
    if (!form.ngayBatDau) errs.ngayBatDau = 'Ngày bắt đầu không được để trống.';
    if (!form.ngayKetThuc) errs.ngayKetThuc = 'Ngày kết thúc không được để trống.';
    if (form.ngayBatDau && form.ngayKetThuc && form.ngayKetThuc < form.ngayBatDau)
      errs.ngayKetThuc = 'Ngày kết thúc phải sau ngày bắt đầu.';
    if (!form.lyDo.trim()) errs.lyDo = 'Lý do không được để trống.';
    if (form.soNgayNghi <= 0) errs.soNgayNghi = 'Số ngày nghỉ phải lớn hơn 0.';
    setErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const handleSubmit = async () => {
    if (!validate()) return;
    setSubmitting(true);
    const ok = await onCreate({
      cccdNhanVien: selectedCccd,
      loaiNghi: form.loaiNghi,
      ngayBatDau: form.ngayBatDau,
      ngayKetThuc: form.ngayKetThuc,
      soNgayNghi: form.soNgayNghi,
      lyDo: form.lyDo,
      taiLieuDinhKem: form.taiLieuDinhKem || undefined,
    });
    setSubmitting(false);
    if (ok) onClose();
  };

  return (
    <div className="dn-modal-overlay" onClick={e => e.target === e.currentTarget && onClose()}>
      <div className="dn-modal">
        <div className="dn-modal-header">
          <h2>Tạo Đơn Xin Nghỉ</h2>
          <button className="dn-modal-close" onClick={onClose}>✕</button>
        </div>

        <div className="dn-modal-body">
          {/* Nhân viên — searchable combobox */}
          <div className="dn-form-row">
            <label className="dn-label">Nhân viên <span className="dn-required">*</span></label>
            <div className="dn-dropdown-select-wrap" ref={dropdownRef}>
              <input
                className={`dn-input${errors.cccd ? ' dn-input--error' : ''}`}
                style={{ width: '100%' }}
                placeholder="-- Chọn nhân viên --"
                value={searchTerm}
                onChange={e => {
                  setSearchTerm(e.target.value);
                  setSelectedCccd('');
                  setIsDropdownOpen(true);
                }}
                onFocus={() => {
                  setSearchTerm('');
                  setSelectedCccd('');
                  setIsDropdownOpen(true);
                }}
                autoComplete="off"
              />
              {isDropdownOpen && (
                <ul className="dn-dropdown-select-list">
                  {filteredEmployees.length > 0
                    ? filteredEmployees.map(emp => (
                      <li
                        key={emp.cccd}
                        className={selectedCccd === emp.cccd ? 'selected' : ''}
                        onClick={() => handleSelectEmployee(emp)}
                      >
                        {emp.hoTen} — {emp.cccd}
                      </li>
                    ))
                    : <li className="dn-empty-option">Không tìm thấy nhân viên</li>}
                </ul>
              )}
            </div>
            {errors.cccd && <span className="dn-error-msg">{errors.cccd}</span>}
          </div>

          {/* Loại nghỉ */}
          <div className="dn-form-row">
            <label className="dn-label">Loại nghỉ <span className="dn-required">*</span></label>
            <select
              className="dn-select"
              value={form.loaiNghi}
              onChange={e => setForm(f => ({ ...f, loaiNghi: e.target.value }))}
            >
              {LOAI_NGHI_OPTIONS.map(opt => (
                <option key={opt.value} value={opt.value}>{opt.label}</option>
              ))}
            </select>
          </div>

          {/* Ngày bắt đầu / Kết thúc */}
          <div className="dn-form-grid">
            <div className="dn-form-row">
              <label className="dn-label">Từ ngày <span className="dn-required">*</span></label>
              <input
                type="date"
                className={`dn-input${errors.ngayBatDau ? ' dn-input--error' : ''}`}
                value={form.ngayBatDau}
                onChange={e => setForm(f => ({ ...f, ngayBatDau: e.target.value }))}
              />
              {errors.ngayBatDau && <span className="dn-error-msg">{errors.ngayBatDau}</span>}
            </div>
            <div className="dn-form-row">
              <label className="dn-label">Đến ngày <span className="dn-required">*</span></label>
              <input
                type="date"
                className={`dn-input${errors.ngayKetThuc ? ' dn-input--error' : ''}`}
                value={form.ngayKetThuc}
                min={form.ngayBatDau}
                onChange={e => setForm(f => ({ ...f, ngayKetThuc: e.target.value }))}
              />
              {errors.ngayKetThuc && <span className="dn-error-msg">{errors.ngayKetThuc}</span>}
            </div>
          </div>

          {/* Số ngày */}
          <div className="dn-form-row">
            <label className="dn-label">Số ngày nghỉ <span className="dn-required">*</span></label>
            <input
              type="number"
              min={0.5}
              step={0.5}
              className={`dn-input${errors.soNgayNghi ? ' dn-input--error' : ''}`}
              value={form.soNgayNghi}
              onChange={e => setForm(f => ({ ...f, soNgayNghi: +e.target.value }))}
            />
            <small className="dn-hint">Tự động tính từ ngày. Hỗ trợ 0.5 (nửa ngày).</small>
            {errors.soNgayNghi && <span className="dn-error-msg">{errors.soNgayNghi}</span>}
          </div>

          {/* Lý do */}
          <div className="dn-form-row">
            <label className="dn-label">Lý do <span className="dn-required">*</span></label>
            <textarea
              className={`dn-textarea${errors.lyDo ? ' dn-input--error' : ''}`}
              rows={3}
              placeholder="Nhập lý do xin nghỉ..."
              value={form.lyDo}
              onChange={e => setForm(f => ({ ...f, lyDo: e.target.value }))}
            />
            {errors.lyDo && <span className="dn-error-msg">{errors.lyDo}</span>}
          </div>

          {/* Tài liệu đính kèm */}
          <div className="dn-form-row">
            <label className="dn-label">Tài liệu đính kèm</label>
            <input
              type="text"
              className="dn-input"
              placeholder="Đường dẫn file (tùy chọn)..."
              value={form.taiLieuDinhKem}
              onChange={e => setForm(f => ({ ...f, taiLieuDinhKem: e.target.value }))}
            />
          </div>
        </div>

        <div className="dn-modal-footer">
          <button className="dn-btn dn-btn--outline" onClick={onClose}>Hủy</button>
          <button className="dn-btn dn-btn--primary" onClick={handleSubmit} disabled={submitting}>
            {submitting ? 'Đang lưu...' : 'Tạo đơn nghỉ'}
          </button>
        </div>
      </div>
    </div>
  );
};
