import React, { useState, useEffect, useCallback } from "react";
import { CauHinhGiamTruDto } from "../types/thueTncn.types";
import { thueTncnApi } from "../api/thueTncnApi";
import { Toast } from "@/components/Toast/Toast";
import "./ThueTncn.css";

const formatCurrency = (val: number) =>
  new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(val);

export const CauHinhGiamTruManagement: React.FC = () => {
  const [cauHinh, setCauHinh] = useState<CauHinhGiamTruDto>({ giamTruBanThan: 11000000, giamTruNguoiPhuThuoc: 4400000 });
  const [loading, setLoading] = useState(true);
  const [toast, setToast] = useState<{ message: string; type: "success" | "error" } | null>(null);

  // GiamTru editing
  const [editGiamTru, setEditGiamTru] = useState<CauHinhGiamTruDto>({ giamTruBanThan: 11000000, giamTruNguoiPhuThuoc: 4400000 });
  const [savingGiamTru, setSavingGiamTru] = useState(false);

  const showToast = (msg: string, type: "success" | "error") => {
    setToast({ message: msg, type });
  };

  const getErrorMessage = (err: any, defaultMsg: string) => {
    const data = err?.response?.data;
    if (data?.Errors && Array.isArray(data.Errors) && data.Errors.length > 0) {
      return data.Errors.join(" ");
    }
    return data?.Message || defaultMsg;
  };

  const loadData = useCallback(async () => {
    setLoading(true);
    try {
      const ghRes = await thueTncnApi.getCauHinhGiamTru();
      if (ghRes) {
        setCauHinh(ghRes);
        setEditGiamTru(ghRes);
      }
    } catch (err: any) {
      showToast(getErrorMessage(err, "Lỗi khi tải dữ liệu."), "error");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { loadData(); }, [loadData]);

  // ---- CauHinhGiamTru ----
  const saveGiamTru = async () => {
    setSavingGiamTru(true);
    try {
      await thueTncnApi.upsertCauHinhGiamTru({
        giamTruBanThan: editGiamTru.giamTruBanThan,
        giamTruNguoiPhuThuoc: editGiamTru.giamTruNguoiPhuThuoc,
        ghiChu: editGiamTru.ghiChu,
      });
      showToast("Cập nhật cấu hình giảm trừ thành công.", "success");
      setCauHinh(editGiamTru);
    } catch (err: any) {
      showToast(getErrorMessage(err, "Có lỗi khi lưu."), "error");
    } finally {
      setSavingGiamTru(false);
    }
  };

  return (
    <div className="tncn-container">
      {/* Page header */}
      <div className="tncn-header">
        <div className="tncn-header-title">
          <h2>Cấu hình Giảm trừ Gia cảnh</h2>
          <p>Thiết lập mức giảm trừ bản thân và người phụ thuộc áp dụng khi tính thuế TNCN</p>
        </div>
      </div>

      <div className="tncn-card">
        <div className="tncn-card-header">
          <div className="tncn-card-header-left">
            <div>
              <h2>Giảm trừ Gia cảnh</h2>
              <p>Mức tiền tính theo tháng áp dụng từ quy định hiện hành</p>
            </div>
          </div>
        </div>

        <div className="tncn-card-body">
          {loading ? (
            <div className="tncn-loader"><div className="tncn-spinner" /></div>
          ) : (
            <>
              <div className="tncn-giamtru-grid">
                <div className="tncn-giamtru-card">
                  <span className="tncn-giamtru-label">Giảm trừ bản thân</span>
                  <span className="tncn-giamtru-value">{formatCurrency(editGiamTru.giamTruBanThan)}</span>
                  <input
                    type="number"
                    className="tncn-giamtru-input"
                    value={editGiamTru.giamTruBanThan}
                    onChange={(e) => setEditGiamTru((p) => ({ ...p, giamTruBanThan: Number(e.target.value) }))}
                    placeholder="VD: 11000000"
                  />
                </div>
                <div className="tncn-giamtru-card">
                  <span className="tncn-giamtru-label">Giảm trừ người phụ thuộc</span>
                  <span className="tncn-giamtru-value">{formatCurrency(editGiamTru.giamTruNguoiPhuThuoc)}</span>
                  <input
                    type="number"
                    className="tncn-giamtru-input"
                    value={editGiamTru.giamTruNguoiPhuThuoc}
                    onChange={(e) => setEditGiamTru((p) => ({ ...p, giamTruNguoiPhuThuoc: Number(e.target.value) }))}
                    placeholder="VD: 4400000"
                  />
                </div>
              </div>

              <div className="tncn-note-row" style={{ marginBottom: "1rem" }}>
                <label style={{ fontSize: "0.8rem", fontWeight: 600, color: "var(--text-secondary)" }}>Ghi chú</label>
                <textarea
                  className="tncn-note-textarea"
                  value={editGiamTru.ghiChu || ""}
                  onChange={(e) => setEditGiamTru((p) => ({ ...p, ghiChu: e.target.value }))}
                  placeholder="Ghi chú thêm (không bắt buộc)..."
                />
              </div>

              <div className="tncn-save-row">
                <button className="tncn-btn tncn-btn-primary" onClick={saveGiamTru} disabled={savingGiamTru}>
                  {savingGiamTru ? "Đang lưu..." : (
                    <>
                      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: 16, height: 16 }}>
                        <path strokeLinecap="round" strokeLinejoin="round" d="m4.5 12.75 6 6 9-13.5" />
                      </svg>
                      Lưu cấu hình
                    </>
                  )}
                </button>
              </div>
            </>
          )}
        </div>
      </div>

      {/* Toast */}
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
