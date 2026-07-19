import React, { useState, useEffect } from "react";
import { workShiftApi } from "../api/workShiftApi";
import { CaLamViec } from "../types";
import { Toast } from "@/components/Toast/Toast";
import { ConfirmModal } from "@/components/ConfirmModal/ConfirmModal";
import { ShiftModal } from "./ShiftModal";
import "./WorkShiftManagement.css";

export const WorkShiftManagement: React.FC = () => {
    const [shifts, setShifts] = useState<CaLamViec[]>([]);
    const [isLoading, setIsLoading] = useState(false);
    const [toast, setToast] = useState<{ message: string; type: "success" | "error" } | null>(null);

    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingShift, setEditingShift] = useState<CaLamViec | null>(null);

    const [confirmDeleteId, setConfirmDeleteId] = useState<string | null>(null);

    const loadShifts = async () => {
        setIsLoading(true);
        try {
            const res = await workShiftApi.getAll();
            if (res.data.succeeded) {
                setShifts(res.data.data);
            } else {
                setToast({ message: res.data.message || "Lỗi khi tải danh sách ca làm việc", type: "error" });
            }
        } catch (error: any) {
            setToast({ message: error.response?.data?.message || "Lỗi kết nối", type: "error" });
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
            if (res.data.succeeded) {
                setToast({ message: "Xóa ca làm việc thành công", type: "success" });
                loadShifts();
            } else {
                setToast({ message: res.data.message || "Xóa thất bại", type: "error" });
            }
        } catch (error: any) {
            setToast({ message: error.response?.data?.message || "Lỗi khi xóa", type: "error" });
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
        <div className="ws-container">
            <div className="ws-header">
                <h2>Cấu hình Ca làm việc</h2>
                <p>Quản lý các ca làm việc và khung giờ nghỉ của nhân viên trong công ty</p>
                <div className="ws-header-actions">
                    <button className="ws-btn-primary" onClick={() => handleOpenModal()}>
                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor">
                            <path fillRule="evenodd" d="M12 3.75a.75.75 0 0 1 .75.75v6.75h6.75a.75.75 0 0 1 0 1.5h-6.75v6.75a.75.75 0 0 1-1.5 0v-6.75H4.5a.75.75 0 0 1 0-1.5h6.75V4.5a.75.75 0 0 1 .75-.75Z" clipRule="evenodd" />
                        </svg>
                        Thêm Ca Làm Việc
                    </button>
                </div>
            </div>

            <div className="ws-content">
                {isLoading ? (
                    <div className="ws-loading">Đang tải dữ liệu...</div>
                ) : (
                    <div className="ws-table-wrapper">
                        <table className="ws-table">
                            <thead>
                                <tr>
                                    <th>Tên ca</th>
                                    <th>Giờ làm việc</th>
                                    <th>Xuyên ngày</th>
                                    <th>Hệ số lương</th>
                                    <th>Giờ nghỉ</th>
                                    <th>Trạng thái</th>
                                    <th className="ws-text-center">Thao tác</th>
                                </tr>
                            </thead>
                            <tbody>
                                {shifts.length === 0 ? (
                                    <tr>
                                        <td colSpan={7} className="ws-empty">Không có dữ liệu</td>
                                    </tr>
                                ) : (
                                    shifts.map(shift => (
                                        <tr key={shift.id}>
                                            <td className="ws-font-medium">{shift.tenCa}</td>
                                            <td>
                                                <span className="ws-time-badge">{shift.gioBatDau.substring(0,5)}</span>
                                                {" - "}
                                                <span className="ws-time-badge">{shift.gioKetThuc.substring(0,5)}</span>
                                            </td>
                                            <td>
                                                {shift.xuyenNgay ? (
                                                    <span className="ws-badge-success">Có</span>
                                                ) : (
                                                    <span className="ws-badge-secondary">Không</span>
                                                )}
                                            </td>
                                            <td>{shift.heSoLuong}</td>
                                            <td>
                                                {shift.khungGioNghis && shift.khungGioNghis.length > 0 ? (
                                                    <ul className="ws-break-list">
                                                        {shift.khungGioNghis.map((b, i) => (
                                                            <li key={i}>
                                                                {b.tenKhoangNghi}: {b.gioBatDau.substring(0,5)} - {b.gioKetThuc.substring(0,5)}
                                                                {b.tinhVaoGioLam && <span className="ws-small-badge">Tính giờ làm</span>}
                                                            </li>
                                                        ))}
                                                    </ul>
                                                ) : (
                                                    <span className="ws-text-muted">Không có</span>
                                                )}
                                            </td>
                                            <td>
                                                {shift.trangThai ? (
                                                    <span className="ws-status-active">Hoạt động</span>
                                                ) : (
                                                    <span className="ws-status-inactive">Ngừng HĐ</span>
                                                )}
                                            </td>
                                            <td>
                                                <div className="ws-action-group">
                                                    <button className="ws-btn-icon" title="Sửa" onClick={() => handleOpenModal(shift)}>
                                                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                                                            <path d="M2.695 14.763l-1.262 3.154a.5.5 0 00.65.65l3.155-1.262a4 4 0 001.343-.885L17.5 5.5a2.121 2.121 0 00-3-3L3.58 13.42a4 4 0 00-.885 1.343z" />
                                                        </svg>
                                                    </button>
                                                    <button className="ws-btn-icon ws-text-danger" title="Xóa" onClick={() => setConfirmDeleteId(shift.id)}>
                                                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                                                            <path fillRule="evenodd" d="M8.75 1A2.75 2.75 0 006 3.75v.443c-.795.077-1.584.176-2.365.298a.75.75 0 10.23 1.482l.149-.022.841 10.518A2.75 2.75 0 007.596 19h4.807a2.75 2.75 0 002.742-2.53l.841-10.52.149.023a.75.75 0 00.23-1.482A41.03 41.03 0 0014 4.193V3.75A2.75 2.75 0 0011.25 1h-2.5zM10 4c.84 0 1.673.025 2.5.075V3.75c0-.69-.56-1.25-1.25-1.25h-2.5c-.69 0-1.25.56-1.25 1.25v.325C8.327 4.025 9.16 4 10 4zM8.58 7.72a.75.75 0 00-1.5.06l.3 7.5a.75.75 0 101.5-.06l-.3-7.5zm4.34.06a.75.75 0 10-1.5-.06l-.3 7.5a.75.75 0 101.5.06l.3-7.5z" clipRule="evenodd" />
                                                        </svg>
                                                    </button>
                                                </div>
                                            </td>
                                        </tr>
                                    ))
                                )}
                            </tbody>
                        </table>
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
        </div>
    );
};
