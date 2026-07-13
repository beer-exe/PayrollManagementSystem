import React, { useState, useEffect, useRef } from 'react';
import type { ChamCongDto, CreateChamCongRequest, UpdateChamCongRequest } from '../types/chamCong.types';
import { employeeApi } from '../../employees/api/employeeApi';
import type { UserProfileDetail } from '../../../types/profile.types';

interface Props {
  editItem: ChamCongDto | null;
  onClose: () => void;
  onCreate: (data: CreateChamCongRequest) => Promise<boolean>;
  onUpdate: (id: string, data: UpdateChamCongRequest) => Promise<boolean>;
}

export const ChamCongFormModal: React.FC<Props> = ({ editItem, onClose, onCreate, onUpdate }) => {
  const isEdit = !!editItem;

  const [cccd, setCccd] = useState(editItem?.cccdNhanVien ?? '');
  const [ngay, setNgay] = useState(editItem?.ngayChamCong ?? '');
  const [gioVao, setGioVao] = useState(editItem?.gioVao ?? '');
  const [gioRa, setGioRa] = useState(editItem?.gioRa ?? '');
  const [ghiChu, setGhiChu] = useState(editItem?.ghiChu ?? '');
  const [submitting, setSubmitting] = useState(false);
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});

  // Employee Dropdown State
  const [employees, setEmployees] = useState<UserProfileDetail[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!isEdit) {
      employeeApi.getEmployees({ PageNumber: 1, PageSize: 1000 })
        .then(res => {
          if (res.data) {
            setEmployees(res.data);
          }
        })
        .catch(console.error);
    }
  }, [isEdit]);

  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(e.target as Node)) {
        setIsDropdownOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const filteredEmployees = employees.filter(emp => 
    `${emp.hoTen} - ${emp.cccd}`.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleSelectEmployee = (emp: UserProfileDetail) => {
    setCccd(emp.cccd);
    setSearchTerm(`${emp.hoTen} - ${emp.cccd}`);
    setIsDropdownOpen(false);
    setFieldErrors(prev => ({ ...prev, cccd: '' }));
  };

  const validate = () => {
    const errs: Record<string, string> = {};
    if (!isEdit && !cccd.trim()) errs.cccd = 'CCCD không được để trống.';
    if (!ngay) errs.ngay = 'Ngày chấm công không được để trống.';
    if (gioVao && gioRa && gioRa <= gioVao) errs.gioRa = 'Giờ ra phải sau giờ vào.';
    return errs;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const errs = validate();
    if (Object.keys(errs).length) { setFieldErrors(errs); return; }
    setFieldErrors({});
    setSubmitting(true);

    let ok: boolean;
    if (isEdit) {
      ok = await onUpdate(editItem!.id, {
        gioVao: gioVao || null,
        gioRa: gioRa || null,
        ghiChu: ghiChu || undefined,
      });
    } else {
      ok = await onCreate({
        cccdNhanVien: cccd.trim(),
        ngayChamCong: ngay,
        gioVao: gioVao || null,
        gioRa: gioRa || null,
        ghiChu: ghiChu || undefined,
      });
    }

    setSubmitting(false);
    if (ok) onClose();
  };

  return (
    <div className="cc-modal-overlay" onClick={e => e.target === e.currentTarget && onClose()}>
      <div className="cc-modal">
        <div className="cc-modal-header">
          <span className="cc-modal-title">{isEdit ? '✏️ Cập nhật chấm công' : '➕ Nhập chấm công thủ công'}</span>
          <button className="cc-modal-close" onClick={onClose}>×</button>
        </div>

        <form onSubmit={handleSubmit}>
          <div className="cc-modal-body">
            {!isEdit && (
              <div className="cc-form-group">
                <label htmlFor="cc-form-cccd">Nhân viên *</label>
                <div className="cc-dropdown-select-wrap" ref={dropdownRef}>
                  <input
                    id="cc-form-cccd"
                    className="cc-form-control"
                    placeholder="-- Chọn nhân viên --"
                    value={searchTerm}
                    onChange={e => {
                      setSearchTerm(e.target.value);
                      setCccd(''); // Clear selected cccd when typing
                      setIsDropdownOpen(true);
                    }}
                    onFocus={() => {
                      setSearchTerm('');
                      setCccd('');
                      setIsDropdownOpen(true);
                    }}
                    autoComplete="off"
                  />
                  {isDropdownOpen && (
                    <ul className="cc-dropdown-select-list">
                      {filteredEmployees.length > 0 ? (
                        filteredEmployees.map(emp => (
                          <li 
                            key={emp.cccd} 
                            className={cccd === emp.cccd ? 'selected' : ''}
                            onClick={() => handleSelectEmployee(emp)}
                          >
                            {emp.hoTen} - {emp.cccd}
                          </li>
                        ))
                      ) : (
                        <li className="cc-empty-option">Không tìm thấy nhân viên</li>
                      )}
                    </ul>
                  )}
                </div>
                {fieldErrors.cccd && <div className="cc-form-error">{fieldErrors.cccd}</div>}
              </div>
            )}

            {isEdit && (
              <div className="cc-form-group">
                <label>Nhân viên</label>
                <input className="cc-form-control" value={`${editItem!.hoTenNhanVien} (${editItem!.cccdNhanVien})`} disabled />
              </div>
            )}

            <div className="cc-form-group">
              <label htmlFor="cc-form-ngay">Ngày chấm công *</label>
              <input
                id="cc-form-ngay"
                type="date"
                className="cc-form-control"
                value={ngay}
                onChange={e => setNgay(e.target.value)}
                max={new Date().toISOString().split('T')[0]}
                disabled={isEdit}
              />
              {fieldErrors.ngay && <div className="cc-form-error">{fieldErrors.ngay}</div>}
            </div>

            <div className="cc-form-row">
              <div className="cc-form-group">
                <label htmlFor="cc-form-gio-vao">Giờ vào</label>
                <input
                  id="cc-form-gio-vao"
                  type="time"
                  className="cc-form-control"
                  value={gioVao}
                  onChange={e => setGioVao(e.target.value)}
                />
              </div>
              <div className="cc-form-group">
                <label htmlFor="cc-form-gio-ra">Giờ ra</label>
                <input
                  id="cc-form-gio-ra"
                  type="time"
                  className="cc-form-control"
                  value={gioRa}
                  onChange={e => setGioRa(e.target.value)}
                />
                {fieldErrors.gioRa && <div className="cc-form-error">{fieldErrors.gioRa}</div>}
              </div>
            </div>

            <div className="cc-form-group">
              <label htmlFor="cc-form-ghi-chu">Ghi chú</label>
              <textarea
                id="cc-form-ghi-chu"
                className="cc-form-control"
                rows={3}
                placeholder="Lý do nhập tay, giải trình..."
                value={ghiChu}
                onChange={e => setGhiChu(e.target.value)}
                style={{ resize: 'vertical' }}
              />
            </div>

            <div style={{ background: '#f5f3ff', borderRadius: 8, padding: '10px 14px', fontSize: 12, color: '#5b21b6' }}>
              💡 Hệ thống sẽ tự động tính số ngày công dựa trên giờ vào/ra và lịch làm việc của công ty.
              Grace period: <strong>15 phút</strong>. Nghỉ trưa: <strong>1 tiếng</strong> (nếu làm &gt; 5 tiếng).
            </div>
          </div>

          <div className="cc-modal-footer">
            <button type="button" className="cc-btn cc-btn--outline" onClick={onClose}>Hủy</button>
            <button type="submit" className="cc-btn cc-btn--primary" disabled={submitting}>
              {submitting ? 'Đang lưu...' : isEdit ? 'Cập nhật' : 'Lưu chấm công'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
