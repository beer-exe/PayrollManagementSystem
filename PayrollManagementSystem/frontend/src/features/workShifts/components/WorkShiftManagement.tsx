import React, { useState, useEffect } from "react";
import { workShiftApi } from "../api/workShiftApi";
import { CaLamViec } from "../types";
import { Toast } from "@/components/Toast/Toast";
import { ConfirmModal } from "@/components/ConfirmModal/ConfirmModal";
import { ShiftModal } from "./ShiftModal";
import { useDataTable } from '../../../hooks/useDataTable';
import { exportToExcel, exportToPdf, ExportColumn } from '../../../utils/exportUtils';
import { SortableHeader } from '../../../components/DataTable/SortableHeader';
import { ExportButtons } from '../../../components/DataTable/ExportButtons';
import "./WorkShiftManagement.css";

import { WorkShiftInstructionModal } from './WorkShiftInstructionModal';

export const WorkShiftManagement: React.FC = () => {
    const [shifts, setShifts] = useState<CaLamViec[]>([]);
    const [isLoading, setIsLoading] = useState(false);
    const [toast, setToast] = useState<{ message: string; type: "success" | "error" } | null>(null);

    const [isModalOpen, setIsModalOpen] = useState(false);
    const [isInstructionModalOpen, setIsInstructionModalOpen] = useState(false);
    const [editingShift, setEditingShift] = useState<CaLamViec | null>(null);

    const [confirmDeleteId, setConfirmDeleteId] = useState<string | null>(null);
    const [openDropdownId, setOpenDropdownId] = useState<string | null>(null);

    const {
        currentData,
        sortKey,
        sortDirection,
        handleSort,
        searchTerm,
        setSearchTerm,
        currentPage,
        totalPages,
        setCurrentPage
    } = useDataTable<CaLamViec>({
        data: shifts,
        initialPageSize: 10,
        searchableFields: ['tenCa']
    });

    const handleExportExcel = () => {
        const columns: ExportColumn<CaLamViec>[] = [
            { header: 'Tên ca', key: 'tenCa' },
            { header: 'Hệ số lương', key: 'heSoLuong' },
            { header: 'Xuyên ngày', key: 'xuyenNgay', render: (item) => item.xuyenNgay ? 'Có' : 'Không' },
            { header: 'Trạng thái', key: 'trangThai', render: (item) => item.trangThai ? 'Hoạt động' : 'Ngừng hoạt động' },
        ];
        exportToExcel(shifts, columns, 'DanhSachCaLamViec');
    };

    const handleExportPdf = () => {
        const columns: ExportColumn<CaLamViec>[] = [
            { header: 'Tên ca', key: 'tenCa' },
            { header: 'Hệ số lương', key: 'heSoLuong' },
            { header: 'Xuyên ngày', key: 'xuyenNgay', render: (item) => item.xuyenNgay ? 'Có' : 'Không' },
            { header: 'Trạng thái', key: 'trangThai', render: (item) => item.trangThai ? 'Hoạt động' : 'Ngừng hoạt động' },
        ];
        exportToPdf(shifts, columns, 'DanhSachCaLamViec', 'Danh Sách Ca Làm Việc');
    };

    useEffect(() => {
        const handleClickOutside = () => setOpenDropdownId(null);
        document.addEventListener("click", handleClickOutside);
        return () => document.removeEventListener("click", handleClickOutside);
    }, []);

    const loadShifts = async () => {
        setIsLoading(true);
        try {
            const res = await workShiftApi.getAll();
            if (res.succeeded) {
                setShifts(res.data);
            } else {
                setToast({ message: res.message || "Lỗi khi tải danh sách ca làm việc", type: "error" });
            }
        } catch (error: any) {
            setToast({ message: error.response?.data?.Message || "Lỗi kết nối", type: "error" });
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        loadShifts();
    }, []);

    const handleDelete = async () => {
        if (!confirmDeleteId) return;
        try {
            const res = await workShiftApi.delete(confirmDeleteId);
            if (res.succeeded) {
                setToast({ message: "Xóa ca làm việc thành công", type: "success" });
                loadShifts();
            } else {
                setToast({ message: res.message || "Xóa thất bại", type: "error" });
            }
        } catch (error: any) {
            setToast({ message: error.response?.data?.Message || "Lỗi khi xóa", type: "error" });
        } finally {
            setConfirmDeleteId(null);
        }
    };

    const handleOpenModal = (shift?: CaLamViec) => {
        if (shift) {
            setEditingShift(shift);
        } else {
            setEditingShift(null);
        }
        setIsModalOpen(true);
    };

    const handleCloseModal = () => {
        setIsModalOpen(false);
        setEditingShift(null);
    };

    return (
        <div className="wsh-container">
            <div className="wsh-header">
                <div className="wsh-header-left">
                    <h2>Cấu hình Ca làm việc</h2>
                    <p>Quản lý các ca làm việc và khung giờ nghỉ của nhân viên trong công ty</p>
                </div>
                <div className="wsh-header-actions" style={{ display: 'flex', gap: '0.75rem' }}>
                    <button className="wsh-btn-secondary wsh-btn-instruction" onClick={() => setIsInstructionModalOpen(true)}>
                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor">
                            <path fillRule="evenodd" d="M2.25 12c0-5.385 4.365-9.75 9.75-9.75s9.75 4.365 9.75 9.75-4.365 9.75-9.75 9.75S2.25 17.385 2.25 12Zm8.706-1.442c1.146-.573 2.437.463 2.126 1.706l-.709 2.836.042-.02a.75.75 0 0 1 .67 1.34l-.04.022c-1.147.573-2.438-.463-2.127-1.706l.71-2.836-.042.02a.75.75 0 1 1-.671-1.34l.041-.022ZM12 9a.75.75 0 1 0 0-1.5.75.75 0 0 0 0 1.5Z" clipRule="evenodd" />
                        </svg>
                        Hướng dẫn cấu hình
                    </button>
                    <button className="wsh-btn-create" onClick={() => handleOpenModal()}>
                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor">
                            <path fillRule="evenodd" d="M12 3.75a.75.75 0 0 1 .75.75v6.75h6.75a.75.75 0 0 1 0 1.5h-6.75v6.75a.75.75 0 0 1-1.5 0v-6.75H4.5a.75.75 0 0 1 0-1.5h6.75V4.5a.75.75 0 0 1 .75-.75Z" clipRule="evenodd" />
                        </svg>
                        Thêm Ca Làm Việc
                    </button>
                </div>
            </div>

            <div className="wsh-filters">
                <div className="wsh-input-wrapper">
                    <svg className="wsh-input-icon" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" d="M21 21l-5.197-5.197m0 0A7.5 7.5 0 105.196 5.196a7.5 7.5 0 0010.607 10.607z" />
                    </svg>
                    <input 
                        type="text" 
                        placeholder="Tìm kiếm theo tên ca..." 
                        value={searchTerm} 
                        onChange={(e) => setSearchTerm(e.target.value)} 
                        className="wsh-input-search" 
                    />
                </div>
                
                <div className="wsh-filters-right">
                    <ExportButtons onExportExcel={handleExportExcel} onExportPdf={handleExportPdf} />
                </div>
            </div>

            <div className="wsh-content">
                {isLoading ? (
                    <div className="wsh-loading">Đang tải dữ liệu...</div>
                ) : (
                    <div className="wsh-table-wrapper">
                        <table className="wsh-table">
                            <thead>
                                <tr>
                                    <SortableHeader label="Tên ca" sortKey="tenCa" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                                    <th>Giờ làm việc</th>
                                    <SortableHeader label="Xuyên ngày" sortKey="xuyenNgay" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                                    <SortableHeader label="Hệ số lương" sortKey="heSoLuong" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                                    <th>Giờ nghỉ</th>
                                    <SortableHeader label="Trạng thái" sortKey="trangThai" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                                    <th className="wsh-text-center">Thao tác</th>
                                </tr>
                            </thead>
                            <tbody>
                                {currentData.length === 0 ? (
                                    <tr>
                                        <td colSpan={7} className="wsh-empty">Không có dữ liệu</td>
                                    </tr>
                                ) : (
                                    currentData.map(shift => (
                                        <tr key={shift.id}>
                                            <td className="wsh-font-medium">{shift.tenCa}</td>
                                            <td>
                                                <span className="wsh-time-badge">{shift.gioBatDau.substring(0,5)}</span>
                                                {" - "}
                                                <span className="wsh-time-badge">{shift.gioKetThuc.substring(0,5)}</span>
                                            </td>
                                            <td>
                                                {shift.xuyenNgay ? (
                                                    <span className="wsh-badge-success">Có</span>
                                                ) : (
                                                    <span className="wsh-badge-secondary">Không</span>
                                                )}
                                            </td>
                                            <td>{shift.heSoLuong}</td>
                                            <td>
                                                {shift.khungGioNghis && shift.khungGioNghis.length > 0 ? (
                                                    <ul className="wsh-break-list">
                                                        {shift.khungGioNghis.map((b, i) => (
                                                            <li key={i}>
                                                                {b.tenKhoangNghi}: {b.gioBatDau.substring(0,5)} - {b.gioKetThuc.substring(0,5)}
                                                                {b.tinhVaoGioLam && <span className="wsh-small-badge">Tính giờ làm</span>}
                                                            </li>
                                                        ))}
                                                    </ul>
                                                ) : (
                                                    <span className="wsh-text-muted">Không có</span>
                                                )}
                                            </td>
                                            <td>
                                                {shift.trangThai ? (
                                                    <span className="wsh-status-active">Hoạt động</span>
                                                ) : (
                                                    <span className="wsh-status-inactive">Ngừng HĐ</span>
                                                )}
                                            </td>
                                            <td className="wsh-td-actions" style={{ position: 'relative' }}>
                                                <div style={{ display: 'flex', justifyContent: 'center' }}>
                                                    <button
                                                        className="wsh-btn-actions"
                                                        onClick={(e) => {
                                                            e.stopPropagation();
                                                            setOpenDropdownId(openDropdownId === shift.id ? null : shift.id);
                                                        }}
                                                        title="Thao tác"
                                                    >
                                                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
                                                            <path strokeLinecap="round" strokeLinejoin="round" d="M12 6.75a.75.75 0 1 1 0-1.5.75.75 0 0 1 0 1.5ZM12 12.75a.75.75 0 1 1 0-1.5.75.75 0 0 1 0 1.5ZM12 18.75a.75.75 0 1 1 0-1.5.75.75 0 0 1 0 1.5Z" />
                                                        </svg>
                                                    </button>
                                                    
                                                    {openDropdownId === shift.id && (
                                                        <div className="wsh-actions-dropdown">
                                                            <button className="wsh-dropdown-item" onClick={() => handleOpenModal(shift)}>
                                                                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" style={{ width: '16px', height: '16px' }}>
                                                                    <path d="M2.695 14.763l-1.262 3.154a.5.5 0 00.65.65l3.155-1.262a4 4 0 001.343-.885L17.5 5.5a2.121 2.121 0 00-3-3L3.58 13.42a4 4 0 00-.885 1.343z" />
                                                                </svg>
                                                                Chỉnh sửa
                                                            </button>
                                                            <button className="wsh-dropdown-item delete" onClick={() => setConfirmDeleteId(shift.id)}>
                                                                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" style={{ width: '16px', height: '16px' }}>
                                                                    <path fillRule="evenodd" d="M8.75 1A2.75 2.75 0 006 3.75v.443c-.795.077-1.584.176-2.365.298a.75.75 0 10.23 1.482l.149-.022.841 10.518A2.75 2.75 0 007.596 19h4.807a2.75 2.75 0 002.742-2.53l.841-10.52.149.023a.75.75 0 00.23-1.482A41.03 41.03 0 0014 4.193V3.75A2.75 2.75 0 0011.25 1h-2.5zM10 4c.84 0 1.673.025 2.5.075V3.75c0-.69-.56-1.25-1.25-1.25h-2.5c-.69 0-1.25.56-1.25 1.25v.325C8.327 4.025 9.16 4 10 4zM8.58 7.72a.75.75 0 00-1.5.06l.3 7.5a.75.75 0 101.5-.06l-.3-7.5zm4.34.06a.75.75 0 10-1.5-.06l-.3 7.5a.75.75 0 101.5.06l.3-7.5z" clipRule="evenodd" />
                                                                </svg>
                                                                Xóa
                                                            </button>
                                                        </div>
                                                    )}
                                                </div>
                                            </td>
                                        </tr>
                                    ))
                                )}
                            </tbody>
                        </table>
                    </div>
                )}

                {totalPages > 0 && !isLoading && (
                    <div className="wsh-pagination" style={{ display: 'flex', justifyContent: 'flex-end', gap: '0.5rem', marginTop: '1rem', padding: '0 1rem 1rem 1rem' }}>
                        <button 
                            className="wsh-btn-secondary" 
                            onClick={() => setCurrentPage(p => p - 1)} 
                            disabled={currentPage === 1 || isLoading}
                            style={{ padding: '0.35rem 0.75rem', borderRadius: '4px' }}
                        >
                            &lt;
                        </button>
                        <div className="wsh-pagination-info" style={{ display: 'flex', alignItems: 'center', color: 'var(--text-secondary)' }}>
                            Trang <span style={{fontWeight: 600, color: 'var(--text-primary)', margin: '0 4px'}}>{currentPage}</span> / <span style={{margin: '0 4px'}}>{totalPages}</span>
                        </div>
                        <button 
                            className="wsh-btn-secondary" 
                            onClick={() => setCurrentPage(p => p + 1)} 
                            disabled={currentPage === totalPages || isLoading}
                            style={{ padding: '0.35rem 0.75rem', borderRadius: '4px' }}
                        >
                            &gt;
                        </button>
                    </div>
                )}
            </div>

            {toast && (
                <Toast
                    message={toast.message}
                    type={toast.type}
                    onClose={() => setToast(null)}
                />
            )}

            <ConfirmModal
                isOpen={!!confirmDeleteId}
                title="Xác nhận xóa"
                message="Bạn có chắc chắn muốn xóa ca làm việc này không? Dữ liệu không thể khôi phục."
                onConfirm={handleDelete}
                onCancel={() => setConfirmDeleteId(null)}
            />

            {isModalOpen && (
                <ShiftModal
                    shift={editingShift}
                    onClose={handleCloseModal}
                    onSaved={() => {
                        handleCloseModal();
                        loadShifts();
                    }}
                />
            )}

            <WorkShiftInstructionModal 
                isOpen={isInstructionModalOpen} 
                onClose={() => setIsInstructionModalOpen(false)} 
            />
        </div>
    );
};
