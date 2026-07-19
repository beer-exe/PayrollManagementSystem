import React, { useState, useEffect } from "react";
import { CaLamViec, KhungGioNghi } from "../types/workShift.types";
import { workShiftApi } from "../api/workShiftApi";
import { ClockTimePicker } from "../../chamCong/components/ClockTimePicker";
import { Toast } from "@/components/Toast/Toast";

interface ShiftModalProps {
    shift: CaLamViec | null;
    onClose: () => void;
    onSaved: () => void;
}

export const ShiftModal: React.FC<ShiftModalProps> = ({ shift, onClose, onSaved }) => {
    const isEdit = !!shift;
    const [isLoading, setIsLoading] = useState(false);
    const [toast, setToast] = useState<{ message: string; type: "success" | "error" } | null>(null);

    const [tenCa, setTenCa] = useState("");
    const [gioBatDau, setGioBatDau] = useState("");
    const [gioKetThuc, setGioKetThuc] = useState("");
    const [xuyenNgay, setXuyenNgay] = useState(false);
    const [heSoLuong, setHeSoLuong] = useState(1.0);
    const [trangThai, setTrangThai] = useState(true);

    const [khungGioNghis, setKhungGioNghis] = useState<KhungGioNghi[]>([]);

    useEffect(() => {
        if (shift) {
            setTenCa(shift.tenCa);
            setGioBatDau(shift.gioBatDau.substring(0, 5));
            setGioKetThuc(shift.gioKetThuc.substring(0, 5));
            setXuyenNgay(shift.xuyenNgay);
            setHeSoLuong(shift.heSoLuong);
            setTrangThai(shift.trangThai);
            
            if (shift.khungGioNghis) {
                setKhungGioNghis(shift.khungGioNghis.map(k => ({
                    ...k,
                    gioBatDau: k.gioBatDau.substring(0, 5),
                    gioKetThuc: k.gioKetThuc.substring(0, 5)
                })));
            }
        }
    }, [shift]);

    const handleAddBreak = () => {
        setKhungGioNghis([...khungGioNghis, {
            tenKhoangNghi: "",
            gioBatDau: "",
            gioKetThuc: "",
            tinhVaoGioLam: false
        }]);
    };

    const handleRemoveBreak = (index: number) => {
        const newBreaks = [...khungGioNghis];
        newBreaks.splice(index, 1);
        setKhungGioNghis(newBreaks);
    };

    const handleBreakChange = (index: number, field: keyof KhungGioNghi, value: any) => {
        const newBreaks = [...khungGioNghis];
        newBreaks[index] = { ...newBreaks[index], [field]: value };
        setKhungGioNghis(newBreaks);
    };

    const validate = () => {
        if (!tenCa) return "Tên ca không được để trống";
        if (!gioBatDau || !gioKetThuc) return "Giờ bắt đầu và kết thúc ca không được để trống";
        if (heSoLuong <= 0) return "Hệ số lương phải lớn hơn 0";

        for (let i = 0; i < khungGioNghis.length; i++) {
            const b = khungGioNghis[i];
            if (!b.tenKhoangNghi) return `Tên khoảng nghỉ thứ ${i+1} không được để trống`;
            if (!b.gioBatDau || !b.gioKetThuc) return `Giờ của khoảng nghỉ thứ ${i+1} không được để trống`;
        }

        return null;
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        
        const error = validate();
        if (error) {
            setToast({ message: error, type: "error" });
            return;
        }

        setIsLoading(true);
        try {
            // Append seconds for TimeSpan parsing
            const formatTime = (time: string) => time.length === 5 ? `${time}:00` : time;

            const payload = {
                tenCa,
                gioBatDau: formatTime(gioBatDau),
                gioKetThuc: formatTime(gioKetThuc),
                xuyenNgay,
                heSoLuong,
                trangThai,
                khungGioNghis: khungGioNghis.map(k => ({
                    ...k,
                    gioBatDau: formatTime(k.gioBatDau),
                    gioKetThuc: formatTime(k.gioKetThuc)
                }))
            };

            let res;
            if (isEdit && shift) {
                res = await workShiftApi.update(shift.id, { ...payload, id: shift.id });
            } else {
                res = await workShiftApi.create(payload);
            }

            if (res.succeeded) {
                setToast({ message: isEdit ? "Cập nhật thành công" : "Thêm mới thành công", type: "success" });
                setTimeout(() => {
                    onSaved();
                }, 1000);
            } else {
                setToast({ message: res.message || "Lỗi khi lưu", type: "error" });
            }
        } catch (error: any) {
            let msg = error.response?.data?.Message || "Lỗi kết nối";
            if (error.response?.data?.errors) {
                msg = error.response.data.errors.join(", ");
            }
            setToast({ message: msg, type: "error" });
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="wsh-modal-overlay">
            <div className="wsh-modal-content">
                <div className="wsh-modal-header">
                    <h2>{isEdit ? "Chỉnh sửa Ca làm việc" : "Thêm mới Ca làm việc"}</h2>
                    <button className="wsh-close-btn" onClick={onClose}>
                        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" style={{ width: '1.25rem', height: '1.25rem' }}>
                            <path strokeLinecap="round" strokeLinejoin="round" d="M6 18 18 6M6 6l12 12" />
                        </svg>
                    </button>
                </div>

                <form className="wsh-form" onSubmit={handleSubmit}>
                    <div className="wsh-form-grid">
                        <div className="wsh-form-group wsh-col-span-2">
                            <label>Tên ca làm việc <span className="wsh-text-danger">*</span></label>
                            <input type="text" value={tenCa} onChange={e => setTenCa(e.target.value)} placeholder="Vd: Ca Hành Chính" className="wsh-input" />
                        </div>
                        
                        <div className="wsh-form-group">
                            <label>Giờ bắt đầu <span className="wsh-text-danger">*</span></label>
                            <ClockTimePicker value={gioBatDau} onChange={setGioBatDau} placeholder="08:00" />
                        </div>

                        <div className="wsh-form-group">
                            <label>Giờ kết thúc <span className="wsh-text-danger">*</span></label>
                            <ClockTimePicker value={gioKetThuc} onChange={setGioKetThuc} placeholder="17:00" />
                        </div>

                        <div className="wsh-form-group">
                            <label>Hệ số lương <span className="wsh-text-danger">*</span></label>
                            <input type="number" step="0.1" value={heSoLuong} onChange={e => setHeSoLuong(parseFloat(e.target.value))} className="wsh-input" />
                        </div>

                        <div className="wsh-form-group wsh-checkbox-group">
                            <label className="wsh-checkbox-label">
                                <input type="checkbox" checked={xuyenNgay} onChange={e => setXuyenNgay(e.target.checked)} />
                                <span className="wsh-checkbox-custom"></span>
                                Làm xuyên ngày (qua 00:00)
                            </label>
                        </div>
                        
                        <div className="wsh-form-group wsh-checkbox-group">
                            <label className="wsh-checkbox-label">
                                <input type="checkbox" checked={trangThai} onChange={e => setTrangThai(e.target.checked)} />
                                <span className="wsh-checkbox-custom"></span>
                                Trạng thái hoạt động
                            </label>
                        </div>
                    </div>

                    <div className="wsh-breaks-section">
                        <div className="wsh-breaks-header">
                            <h3>Khung giờ nghỉ</h3>
                            <button type="button" className="wsh-btn-outline" onClick={handleAddBreak}>
                                + Thêm giờ nghỉ
                            </button>
                        </div>

                        {khungGioNghis.length === 0 ? (
                            <p className="wsh-empty-text">Chưa có khung giờ nghỉ nào được cấu hình.</p>
                        ) : (
                            <div className="wsh-breaks-list">
                                {khungGioNghis.map((b, index) => (
                                    <div key={index} className="wsh-break-item">
                                        <div className="wsh-form-group">
                                            <input type="text" placeholder="Tên giờ nghỉ" value={b.tenKhoangNghi} onChange={e => handleBreakChange(index, "tenKhoangNghi", e.target.value)} className="wsh-input" />
                                        </div>
                                        <div className="wsh-form-group">
                                            <ClockTimePicker value={b.gioBatDau} onChange={val => handleBreakChange(index, "gioBatDau", val)} placeholder="12:00" />
                                        </div>
                                        <div className="wsh-form-group">
                                            <ClockTimePicker value={b.gioKetThuc} onChange={val => handleBreakChange(index, "gioKetThuc", val)} placeholder="13:00" />
                                        </div>
                                        <div className="wsh-form-group wsh-checkbox-center">
                                            <label className="wsh-checkbox-label" title="Có tính thời gian nghỉ này vào tổng thời gian làm việc không?">
                                                <input type="checkbox" checked={b.tinhVaoGioLam} onChange={e => handleBreakChange(index, "tinhVaoGioLam", e.target.checked)} />
                                                <span className="wsh-checkbox-custom"></span>
                                                Tính giờ làm
                                            </label>
                                        </div>
                                        <button type="button" className="wsh-btn-icon-danger" onClick={() => handleRemoveBreak(index)}>
                                            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                                                <path fillRule="evenodd" d="M8.75 1A2.75 2.75 0 006 3.75v.443c-.795.077-1.584.176-2.365.298a.75.75 0 10.23 1.482l.149-.022.841 10.518A2.75 2.75 0 007.596 19h4.807a2.75 2.75 0 002.742-2.53l.841-10.52.149.023a.75.75 0 00.23-1.482A41.03 41.03 0 0014 4.193V3.75A2.75 2.75 0 0011.25 1h-2.5zM10 4c.84 0 1.673.025 2.5.075V3.75c0-.69-.56-1.25-1.25-1.25h-2.5c-.69 0-1.25.56-1.25 1.25v.325C8.327 4.025 9.16 4 10 4zM8.58 7.72a.75.75 0 00-1.5.06l.3 7.5a.75.75 0 101.5-.06l-.3-7.5zm4.34.06a.75.75 0 10-1.5-.06l-.3 7.5a.75.75 0 101.5.06l.3-7.5z" clipRule="evenodd" />
                                            </svg>
                                        </button>
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>

                    <div className="wsh-modal-footer">
                        <button type="button" className="wsh-btn-secondary" onClick={onClose} disabled={isLoading}>
                            Hủy bỏ
                        </button>
                        <button type="submit" className="wsh-btn-primary" disabled={isLoading}>
                            {isLoading ? "Đang lưu..." : "Lưu thay đổi"}
                        </button>
                    </div>
                </form>
            </div>
            
            {toast && (
                <Toast
                    message={toast.message}
                    type={toast.type}
                    onClose={() => setToast(null)}
                />
            )}
        </div>
    );
};
