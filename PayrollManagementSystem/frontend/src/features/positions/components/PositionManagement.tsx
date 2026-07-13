import React, { useEffect, useState } from "react";
import { usePositions } from "../hooks/usePositions";
import { PositionDto } from "../types/position.types";
import { useJobGrades } from "../../jobGrades/hooks/useJobGrades";
import { departmentApi } from "../../departments/api/departmentApi";
import { DepartmentDto } from "../../departments/types/department.types";
import { PositionModal } from "./PositionModal";
import './PositionManagement.css';

export const PositionManagement: React.FC = () => {
  const {
    positions,
    loading,
    fetchPositions,
    createPosition,
    updatePosition,
    toggleStatus,
  } = usePositions();

  const { jobGrades, fetchJobGrades: fetchJobGradesData } = useJobGrades();

  const [searchTerm, setSearchTerm] = useState("");
  const [statusFilter, setStatusFilter] = useState<string | undefined>(undefined);
  const [selectedDepartmentId, setSelectedDepartmentId] = useState<string | undefined>(undefined);
  
  const [departments, setDepartments] = useState<DepartmentDto[]>([]);

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingPos, setEditingPos] = useState<PositionDto | null>(null);
  
  // Dropdown state for actions
  const [activeDropdown, setActiveDropdown] = useState<string | null>(null);

  // Pagination states
  const [currentPage, setCurrentPage] = useState(1);
  const pageSize = 10;

  useEffect(() => {
    fetchPositions(searchTerm, statusFilter, selectedDepartmentId);
    setCurrentPage(1); // Reset page on filter change
  }, [searchTerm, statusFilter, selectedDepartmentId, fetchPositions]);

  useEffect(() => {
    fetchJobGradesData();
    departmentApi.getDepartments().then(res => {
      if (res.succeeded) setDepartments(res.data);
    });
  }, [fetchJobGradesData]);

  const handleOpenModal = (record?: PositionDto) => {
    setEditingPos(record || null);
    setIsModalOpen(true);
  };

  const handleModalSubmit = async (isEdit: boolean, idChucVu: string, data: any) => {
    let success = false;
    if (isEdit) {
      success = await updatePosition(idChucVu, {
        ...data,
        idChucVu: idChucVu,
      });
    } else {
      success = await createPosition(data);
    }

    if (success) {
      fetchPositions(searchTerm, statusFilter, selectedDepartmentId);
    }
    return success;
  };

  const handleToggleStatus = (record: PositionDto) => {
    const isActivating = record.trangThai !== "HOAT_DONG";
    const msg = isActivating 
      ? `Bạn có muốn kích hoạt lại chức vụ "${record.tenChucVu}"?`
      : `Bạn có chắc muốn vô hiệu hóa chức vụ "${record.tenChucVu}"?`;

    if (window.confirm(msg)) {
      toggleStatus(record.idChucVu).then(success => {
        if (success) fetchPositions(searchTerm, statusFilter, selectedDepartmentId);
      });
    }
    setActiveDropdown(null);
  };

  // Handle clicking outside to close dropdown
  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (activeDropdown && !(e.target as Element).closest('.pos-actions')) {
        setActiveDropdown(null);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, [activeDropdown]);

  // Pagination logic
  const totalItems = positions.length;
  const totalPages = Math.ceil(totalItems / pageSize);
  const currentData = positions.slice((currentPage - 1) * pageSize, currentPage * pageSize);

  const handlePrevPage = () => {
    if (currentPage > 1) setCurrentPage(p => p - 1);
  };

  const handleNextPage = () => {
    if (currentPage < totalPages) setCurrentPage(p => p + 1);
  };

  return (
    <div className="pos-container">
      <div className="pos-header">
        <div className="pos-header-title">
          <h2>Danh Mục Chức Vụ</h2>
          <p>Quản lý các chức vụ và mô tả công việc (3P)</p>
        </div>
        <button
          className="pos-btn pos-btn-primary"
          onClick={() => handleOpenModal()}
          disabled={!selectedDepartmentId}
          title={!selectedDepartmentId ? "Vui lòng chọn phòng ban để thêm chức vụ" : ""}
        >
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
          </svg>
          Thêm Chức Vụ
        </button>
      </div>

      <div className="pos-controls-wrapper">
        <div className="pos-filters">
          <div className="pos-input-wrapper">
            <svg className="pos-input-icon" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: '1.1rem', height: '1.1rem' }}>
              <path strokeLinecap="round" strokeLinejoin="round" d="M21 21l-5.197-5.197m0 0A7.5 7.5 0 105.196 5.196a7.5 7.5 0 0010.607 10.607z" />
            </svg>
            <input
              type="text"
              placeholder="Tìm theo Mã, Tên chức vụ..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pos-input"
            />
          </div>

          <select
            value={selectedDepartmentId || ""}
            onChange={(e) => setSelectedDepartmentId(e.target.value || undefined)}
            className="pos-select dept"
          >
            <option value="">Lọc theo phòng ban</option>
            {departments?.map((d: any) => (
              <option key={d.idPb} value={d.idPb}>{d.tenPb}</option>
            ))}
          </select>

          <select
            value={statusFilter || ""}
            onChange={(e) => setStatusFilter(e.target.value || undefined)}
            className="pos-select status"
          >
            <option value="">Lọc theo trạng thái</option>
            <option value="HOAT_DONG">Đang hoạt động</option>
            <option value="NGUNG_HOAT_DONG">Ngừng hoạt động</option>
          </select>
        </div>

        <div className="pos-table-container custom-scrollbar">
          {loading ? (
            <div className="pos-loader">
              <div className="pos-spinner"></div>
            </div>
          ) : currentData.length > 0 ? (
            <table className="pos-table">
              <thead>
                <tr>
                  <th>Mã Chức Vụ</th>
                  <th>Tên Chức Vụ</th>
                  <th>Phòng Ban</th>
                  <th>Quản Lý Trực Tiếp</th>
                  <th>Mô Tả Công Việc</th>
                  <th>Ngạch Lương</th>
                  <th style={{ textAlign: 'center' }}>Trạng Thái</th>
                  <th style={{ textAlign: 'right' }}>Hành Động</th>
                </tr>
              </thead>
              <tbody>
                {currentData.map(record => (
                  <tr key={record.idChucVu}>
                    <td className="mono">{record.idChucVu}</td>
                    <td style={{ fontWeight: 600, color: '#111827' }}>{record.tenChucVu}</td>
                    <td>{record.tenPhongBan || <span style={{ color: '#9ca3af' }}>Chưa gán</span>}</td>
                    <td>{record.tenChucVuQuanLy || '-'}</td>
                    <td><div className="truncate" title={record.moTaCongViec || ''}>{record.moTaCongViec}</div></td>
                    <td>
                      {record.tenNgachLuong ? (
                        <span className="pos-badge pos-badge-blue">{record.tenNgachLuong}</span>
                      ) : (
                        <span style={{ color: '#9ca3af' }}>Chưa gán</span>
                      )}
                    </td>
                    <td style={{ textAlign: 'center' }}>
                      {record.trangThai === "HOAT_DONG" ? (
                        <span className="pos-badge pos-badge-success">{record.tenTrangThai}</span>
                      ) : (
                        <span className="pos-badge pos-badge-gray">{record.tenTrangThai}</span>
                      )}
                    </td>
                    <td>
                      <div className="pos-actions">
                        <button 
                          className="pos-btn-actions"
                          onClick={(e) => {
                            e.stopPropagation();
                            setActiveDropdown(activeDropdown === record.idChucVu ? null : record.idChucVu);
                          }}
                        >
                          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
                            <path strokeLinecap="round" strokeLinejoin="round" d="M12 6.75a.75.75 0 110-1.5.75.75 0 010 1.5zM12 12.75a.75.75 0 110-1.5.75.75 0 010 1.5zM12 18.75a.75.75 0 110-1.5.75.75 0 010 1.5z" />
                          </svg>
                        </button>
                        
                        {activeDropdown === record.idChucVu && (
                          <div className="pos-actions-dropdown">
                            <button 
                              className="pos-dropdown-item" 
                              onClick={() => {
                                handleOpenModal(record);
                                setActiveDropdown(null);
                              }}
                            >
                              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1rem', height: '1rem' }}>
                                <path strokeLinecap="round" strokeLinejoin="round" d="M16.862 4.487l1.687-1.688a1.875 1.875 0 112.652 2.652L6.832 19.82a4.5 4.5 0 01-1.897 1.13l-2.685.8.8-2.685a4.5 4.5 0 011.13-1.897L16.863 4.487zm0 0L19.5 7.125" />
                              </svg>
                              Sửa chức vụ
                            </button>
                            <button 
                              className={`pos-dropdown-item ${record.trangThai === "HOAT_DONG" ? "warning" : "success"}`}
                              onClick={() => handleToggleStatus(record)}
                            >
                              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1rem', height: '1rem' }}>
                                <path strokeLinecap="round" strokeLinejoin="round" d="M16.023 9.348h4.992v-.001M2.985 19.644v-4.992m0 0h4.992m-4.993 0l3.181 3.183a8.25 8.25 0 0013.803-3.7M4.031 9.865a8.25 8.25 0 0113.803-3.7l3.181 3.182m0-4.991v4.99" />
                              </svg>
                              {record.trangThai === "HOAT_DONG" ? "Vô hiệu hóa" : "Kích hoạt"}
                            </button>
                          </div>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            <div className="pos-empty">
              <p>Không tìm thấy chức vụ nào phù hợp.</p>
            </div>
          )}
        </div>

        {totalPages > 0 && (
          <div className="pos-pagination">
            <button 
              className="pos-btn pos-btn-secondary" 
              onClick={handlePrevPage} 
              disabled={currentPage === 1 || loading}
              style={{ padding: '0.35rem 0.75rem' }}
            >
              Trước
            </button>
            <div className="pos-pagination-info">
              Trang <span>{currentPage}</span> / <span>{totalPages}</span>
            </div>
            <button 
              className="pos-btn pos-btn-secondary" 
              onClick={handleNextPage} 
              disabled={currentPage === totalPages || loading}
              style={{ padding: '0.35rem 0.75rem' }}
            >
              Sau
            </button>
          </div>
        )}
      </div>

      <PositionModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        editingPos={editingPos}
        departments={departments}
        jobGrades={jobGrades}
        positions={positions}
        selectedDepartmentId={selectedDepartmentId}
        onSubmit={handleModalSubmit}
      />
    </div>
  );
};
