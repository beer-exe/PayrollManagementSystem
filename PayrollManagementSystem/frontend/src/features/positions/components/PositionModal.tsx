import React, { useEffect, useState } from 'react';
import { PositionDto, CreatePositionDto, UpdatePositionDto } from '../types/position.types';
import { DepartmentDto } from '../../departments/types/department.types';

interface JobGradeDto {
  idNgachLuong: string;
  tenNgachLuong: string;
  trangThai: number;
}

interface Props {
  isOpen: boolean;
  onClose: () => void;
  editingPos: PositionDto | null;
  departments: DepartmentDto[];
  jobGrades: JobGradeDto[];
  positions: PositionDto[];
  selectedDepartmentId?: string;
  onSubmit: (isEdit: boolean, idChucVu: string, data: any) => Promise<boolean>;
}

export const PositionModal: React.FC<Props> = ({
  isOpen,
  onClose,
  editingPos,
  departments,
  jobGrades,
  positions,
  selectedDepartmentId,
  onSubmit,
}) => {
  const [formData, setFormData] = useState({
    idChucVu: '',
    tenChucVu: '',
    moTaCongViec: '',
    idNgachLuong: '',
    idPhongBan: '',
    idChucVuQuanLy: '',
  });

  const [hasManager, setHasManager] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (isOpen) {
      if (editingPos) {
        setFormData({
          idChucVu: editingPos.idChucVu,
          tenChucVu: editingPos.tenChucVu,
          moTaCongViec: editingPos.moTaCongViec || '',
          idNgachLuong: editingPos.idNgachLuong || '',
          idPhongBan: editingPos.idPhongBan,
          idChucVuQuanLy: editingPos.idChucVuQuanLy || '',
        });
        setHasManager(!!editingPos.idChucVuQuanLy);
      } else {
        setFormData({
          idChucVu: '',
          tenChucVu: '',
          moTaCongViec: '',
          idNgachLuong: '',
          idPhongBan: selectedDepartmentId || '',
          idChucVuQuanLy: '',
        });
        setHasManager(false);
      }
      setErrors({});
    }
  }, [isOpen, editingPos, selectedDepartmentId]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  const handleCheckboxChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const checked = e.target.checked;
    setHasManager(checked);
    if (!checked) {
      setFormData(prev => ({ ...prev, idChucVuQuanLy: '' }));
      if (errors.idChucVuQuanLy) {
        setErrors(prev => ({ ...prev, idChucVuQuanLy: '' }));
      }
    }
  };

  const validate = () => {
    const newErrors: Record<string, string> = {};
    if (!formData.idChucVu.trim()) newErrors.idChucVu = 'Vui lòng nhập mã chức vụ!';
    if (!formData.tenChucVu.trim()) newErrors.tenChucVu = 'Vui lòng nhập tên chức vụ!';
    if (!formData.idPhongBan) newErrors.idPhongBan = 'Vui lòng chọn phòng ban!';
    if (hasManager && !formData.idChucVuQuanLy) newErrors.idChucVuQuanLy = 'Vui lòng chọn chức vụ quản lý trực tiếp!';

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validate()) return;

    setIsSubmitting(true);
    
    // Clean data before submitting
    const submitData = {
      idChucVu: formData.idChucVu,
      tenChucVu: formData.tenChucVu,
      moTaCongViec: formData.moTaCongViec || null,
      idNgachLuong: formData.idNgachLuong || null,
      idPhongBan: formData.idPhongBan,
      idChucVuQuanLy: hasManager ? (formData.idChucVuQuanLy || null) : null,
    };

    const success = await onSubmit(!!editingPos, formData.idChucVu, submitData);
    setIsSubmitting(false);
    if (success) {
      onClose();
    }
  };

  if (!isOpen) return null;

  return (
    <div className="pos-modal-overlay">
      <div className="pos-modal">
        <div className="pos-modal-header">
          <h3 className="pos-modal-title">
            {editingPos ? "Cập Nhật Chức Vụ" : "Thêm Mới Chức Vụ"}
          </h3>
          <button className="pos-modal-close" onClick={onClose} disabled={isSubmitting}>
            &times;
          </button>
        </div>

        <div className="pos-modal-body custom-scrollbar">
          <form id="position-form" onSubmit={handleSubmit}>
            <div className="pos-form-group">
              <label className="pos-form-label">
                Mã Chức Vụ <span className="required">*</span>
              </label>
              <input
                type="text"
                name="idChucVu"
                value={formData.idChucVu}
                onChange={handleChange}
                disabled={!!editingPos}
                className="pos-form-input"
                placeholder="VD: CV_TRUONGPHONG"
              />
              {errors.idChucVu && <span className="pos-form-error">{errors.idChucVu}</span>}
            </div>

            <div className="pos-form-group">
              <label className="pos-form-label">
                Tên Chức Vụ <span className="required">*</span>
              </label>
              <input
                type="text"
                name="tenChucVu"
                value={formData.tenChucVu}
                onChange={handleChange}
                className="pos-form-input"
                placeholder="VD: Trưởng Phòng"
              />
              {errors.tenChucVu && <span className="pos-form-error">{errors.tenChucVu}</span>}
            </div>

            <div className="pos-form-group">
              <label className="pos-form-label">Mô Tả Công Việc</label>
              <textarea
                name="moTaCongViec"
                value={formData.moTaCongViec}
                onChange={handleChange}
                className="pos-form-textarea"
                placeholder="Mô tả sơ lược về công việc, trách nhiệm..."
              />
            </div>

            <div className="pos-form-group">
              <label className="pos-form-label">Ngạch Lương</label>
              <select
                name="idNgachLuong"
                value={formData.idNgachLuong}
                onChange={handleChange}
                className="pos-form-select"
              >
                <option value="">Chọn ngạch lương</option>
                {jobGrades?.filter(g => g.trangThai === 1).map(g => (
                  <option key={g.idNgachLuong} value={g.idNgachLuong}>
                    {g.tenNgachLuong}
                  </option>
                ))}
              </select>
            </div>

            <div className="pos-form-group">
              <label className="pos-form-label">
                Phòng Ban <span className="required">*</span>
              </label>
              <select
                name="idPhongBan"
                value={formData.idPhongBan}
                onChange={handleChange}
                className="pos-form-select"
                disabled={true} // Theo thiết kế gốc, field này disabled
              >
                <option value="">Chọn phòng ban</option>
                {departments?.map(d => (
                  <option key={d.idPb} value={d.idPb}>
                    {d.tenPb}
                  </option>
                ))}
              </select>
              {errors.idPhongBan && <span className="pos-form-error">{errors.idPhongBan}</span>}
            </div>

            <div className="pos-form-group" style={{ marginBottom: hasManager ? '0.5rem' : '1.25rem' }}>
              <label className="pos-checkbox-wrapper">
                <input
                  type="checkbox"
                  checked={hasManager}
                  onChange={handleCheckboxChange}
                />
                <span className="pos-checkbox-label">Chức vụ này có báo cáo cho Quản lý trực tiếp?</span>
              </label>
            </div>

            {hasManager && (
              <div className="pos-form-group" style={{ marginTop: '0' }}>
                <label className="pos-form-label">
                  Quản Lý Trực Tiếp (Báo cáo cho) <span className="required">*</span>
                </label>
                <select
                  name="idChucVuQuanLy"
                  value={formData.idChucVuQuanLy}
                  onChange={handleChange}
                  className="pos-form-select"
                >
                  <option value="">Chọn chức vụ quản lý</option>
                  {positions?.filter(p => p.idChucVu !== editingPos?.idChucVu && p.trangThai === "HOAT_DONG").map(p => (
                    <option key={p.idChucVu} value={p.idChucVu}>
                      {p.tenChucVu} - {p.tenPhongBan}
                    </option>
                  ))}
                </select>
                {errors.idChucVuQuanLy && <span className="pos-form-error">{errors.idChucVuQuanLy}</span>}
              </div>
            )}
          </form>
        </div>

        <div className="pos-modal-footer">
          <button type="button" onClick={onClose} className="pos-btn pos-btn-secondary" disabled={isSubmitting}>
            Hủy bỏ
          </button>
          <button type="submit" form="position-form" className="pos-btn pos-btn-primary" disabled={isSubmitting}>
            {isSubmitting ? 'Đang lưu...' : 'Lưu lại'}
          </button>
        </div>
      </div>
    </div>
  );
};
