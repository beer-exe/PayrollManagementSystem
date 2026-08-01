import React, { useState, useEffect, useCallback } from "react";
import { BacThueDto, CreateBacThueRequest } from "../types/thueTncn.types";
import { thueTncnApi } from "../api/thueTncnApi";
import { useDataTable } from "@/hooks/useDataTable";
import { SortableHeader } from "@/components/DataTable/SortableHeader";
import { ExportButtons } from "@/components/DataTable/ExportButtons";
import { exportToExcel, exportToPdf, ExportColumn } from "@/utils/exportUtils";
import { Toast } from "@/components/Toast/Toast";
import "./ThueTncn.css";

const formatCurrency = (val: number) =>
  new Intl.NumberFormat("vi-VN", { style: "currency", currency: "VND" }).format(val);

const formatPercent = (val: number) => `${val}%`;

interface EditRow {
  tuGia: string;
  denGia: string;
  thueSuat: string;
}

export const BangThueLuyTienManagement: React.FC = () => {
  const [bacThueList, setBacThueList] = useState<BacThueDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [toast, setToast] = useState<{ message: string; type: "success" | "error" } | null>(null);

  const {
    searchTerm,
    setSearchTerm,
    sortKey,
    sortDirection,
    handleSort,
    currentPage,
    currentData: paginatedItems,
    totalPages,
    setCurrentPage: handlePageChange,
    allFilteredAndSortedData
  } = useDataTable<BacThueDto>({
    data: bacThueList,
    searchableFields: ['bac', 'tuGia', 'denGia', 'thueSuat'],
  });

  const handleExportExcel = () => {
    const columns: ExportColumn<BacThueDto>[] = [
      { header: 'Bậc', key: 'bac' },
      { header: 'Từ (VNĐ)', key: 'tuGia' },
      { header: 'Đến (VNĐ)', key: 'denGia' },
      { header: 'Thuế suất (%)', key: 'thueSuat' }
    ];
    exportToExcel(allFilteredAndSortedData, columns, `Bang_Thue_TNCN.xlsx`);
  };

  const handleExportPdf = () => {
    const columns: ExportColumn<BacThueDto>[] = [
      { header: 'Bậc', key: 'bac' },
      { header: 'Từ (VNĐ)', key: 'tuGia' },
      { header: 'Đến (VNĐ)', key: 'denGia' },
      { header: 'Thuế suất (%)', key: 'thueSuat' }
    ];
    exportToPdf(allFilteredAndSortedData, columns, `Bang_Thue_TNCN.pdf`, `BẢNG THUẾ LŨY TIẾN TNCN`);
  };

  // Inline edit state
  const [editingId, setEditingId] = useState<string | null>(null);
  const [editRow, setEditRow] = useState<EditRow>({ tuGia: "", denGia: "", thueSuat: "" });

  // Add form state
  const [showAddForm, setShowAddForm] = useState(false);
  const [addForm, setAddForm] = useState<CreateBacThueRequest>({ bac: 0, tuGia: 0, denGia: null, thueSuat: 0 });
  const [saving, setSaving] = useState(false);

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
      const btRes = await thueTncnApi.getBacThueList();
      setBacThueList(btRes || []);
    } catch (err: any) {
      showToast(getErrorMessage(err, "Lỗi khi tải dữ liệu."), "error");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { loadData(); }, [loadData]);

  // ---- BacThue inline edit ----
  const startEdit = (bt: BacThueDto) => {
    setEditingId(bt.idBacThue);
    setEditRow({
      tuGia: String(bt.tuGia),
      denGia: bt.denGia !== null ? String(bt.denGia) : "",
      thueSuat: String(bt.thueSuat),
    });
  };

  const cancelEdit = () => setEditingId(null);

  const saveEdit = async (bt: BacThueDto) => {
    setSaving(true);
    try {
      await thueTncnApi.updateBacThue(bt.idBacThue, {
        tuGia: Number(editRow.tuGia),
        denGia: editRow.denGia !== "" ? Number(editRow.denGia) : null,
        thueSuat: Number(editRow.thueSuat),
        isActive: bt.isActive,
      });
      showToast("Cập nhật bậc thuế thành công.", "success");
      setEditingId(null);
      await loadData();
    } catch (err: any) {
      showToast(getErrorMessage(err, "Có lỗi khi cập nhật."), "error");
    } finally {
      setSaving(false);
    }
  };

  const deleteBacThue = async (id: string, bac: number) => {
    if (!confirm(`Bạn có chắc muốn xóa Bậc ${bac}?`)) return;
    try {
      await thueTncnApi.deleteBacThue(id);
      showToast("Xóa bậc thuế thành công.", "success");
      await loadData();
    } catch (err: any) {
      showToast(getErrorMessage(err, "Có lỗi khi xóa."), "error");
    }
  };

  const submitAddForm = async () => {
    setSaving(true);
    try {
      await thueTncnApi.createBacThue(addForm);
      showToast("Thêm bậc thuế thành công.", "success");
      setShowAddForm(false);
      setAddForm({ bac: 0, tuGia: 0, denGia: null, thueSuat: 0 });
      await loadData();
    } catch (err: any) {
      showToast(getErrorMessage(err, "Có lỗi khi thêm."), "error");
    } finally {
      setSaving(false);
    }
  };

  const describeRange = (bt: BacThueDto) => {
    if (bt.tuGia === 0 && bt.denGia !== null)
      return `Đến ${formatCurrency(bt.denGia)}`;
    if (bt.denGia === null)
      return `Trên ${formatCurrency(bt.tuGia)}`;
    return `${formatCurrency(bt.tuGia)} – ${formatCurrency(bt.denGia)}`;
  };

  return (
    <div className="tncn-container">
      {/* Page header */}
      <div className="tncn-header">
        <div className="tncn-header-title">
          <h2>Cấu hình Bảng Tính Thuế Lũy Tiến</h2>
          <p>Quản lý các bậc thuế thu nhập cá nhân</p>
        </div>
      </div>

      <div className="tncn-card">
        <div className="tncn-card-header">
          <div className="tncn-card-header-left">
            <div>
              <h2>Bảng Tính Thuế Lũy Tiến</h2>
              <p>Danh sách cấu hình các bậc thuế</p>
            </div>
          </div>
          <div style={{ display: 'flex', gap: '0.75rem', alignItems: 'center' }}>
            <input
              type="text"
              placeholder="Tìm kiếm bậc thuế..."
              className="tncn-inline-input"
              style={{ width: '200px', margin: 0, height: '36px' }}
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
            <ExportButtons onExportExcel={handleExportExcel} onExportPdf={handleExportPdf} />
            <button className="tncn-btn tncn-btn-primary" onClick={() => setShowAddForm(!showAddForm)}>
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: 16, height: 16 }}>
                <path strokeLinecap="round" strokeLinejoin="round" d="M12 4.5v15m7.5-7.5h-15" />
              </svg>
              Thêm Bậc
            </button>
          </div>
        </div>

        <div className="tncn-card-body">
          {loading ? (
            <div className="tncn-loader"><div className="tncn-spinner" /></div>
          ) : (
            <>
              <div className="tncn-table-container">
                <table className="tncn-table">
                  <thead>
                    <tr>
                      <SortableHeader label="Bậc" sortKey="bac" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                      <th>Thu nhập tính thuế/tháng</th>
                      <SortableHeader label="Từ (VNĐ)" sortKey="tuGia" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                      <SortableHeader label="Đến (VNĐ)" sortKey="denGia" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                      <SortableHeader label="Thuế suất (%)" sortKey="thueSuat" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} />
                      <th>Thao tác</th>
                    </tr>
                  </thead>
                  <tbody>
                    {(!paginatedItems || paginatedItems.length === 0) && (
                      <tr>
                        <td colSpan={6} className="tncn-empty">Chưa có dữ liệu.</td>
                      </tr>
                    )}
                    {(paginatedItems || []).map((bt) =>
                      editingId === bt.idBacThue ? (
                        <tr key={bt.idBacThue}>
                          <td><span className="tncn-bac-badge">{bt.bac}</span></td>
                          <td style={{ color: "var(--text-secondary)", fontSize: "0.82rem" }}>{describeRange(bt)}</td>
                          <td>
                            <input
                              type="number"
                              className="tncn-inline-input"
                              value={editRow.tuGia}
                              onChange={(e) => setEditRow((p) => ({ ...p, tuGia: e.target.value }))}
                            />
                          </td>
                          <td>
                            <input
                              type="number"
                              className="tncn-inline-input"
                              placeholder="Để trống = Không giới hạn"
                              value={editRow.denGia}
                              onChange={(e) => setEditRow((p) => ({ ...p, denGia: e.target.value }))}
                            />
                          </td>
                          <td>
                            <input
                              type="number"
                              className="tncn-inline-input"
                              value={editRow.thueSuat}
                              onChange={(e) => setEditRow((p) => ({ ...p, thueSuat: e.target.value }))}
                            />
                          </td>
                          <td>
                            <div className="tncn-actions">
                              <button className="tncn-btn tncn-btn-primary tncn-btn-sm" onClick={() => saveEdit(bt)} disabled={saving}>Lưu</button>
                              <button className="tncn-btn tncn-btn-outline tncn-btn-sm" onClick={cancelEdit}>Hủy</button>
                            </div>
                          </td>
                        </tr>
                      ) : (
                        <tr key={bt.idBacThue}>
                          <td><span className="tncn-bac-badge">{bt.bac}</span></td>
                          <td>{describeRange(bt)}</td>
                          <td>{formatCurrency(bt.tuGia)}</td>
                          <td>{bt.denGia !== null ? formatCurrency(bt.denGia) : <em style={{ color: "var(--text-secondary)" }}>Không giới hạn</em>}</td>
                          <td><span className="tncn-rate-pill">{formatPercent(bt.thueSuat)}</span></td>
                          <td>
                            <div className="tncn-actions">
                              <button className="tncn-btn-text" onClick={() => startEdit(bt)}>Sửa</button>
                              <button className="tncn-btn-text danger" onClick={() => deleteBacThue(bt.idBacThue, bt.bac)}>Xóa</button>
                            </div>
                          </td>
                        </tr>
                      )
                    )}
                  </tbody>
                </table>
              </div>

              <div style={{ padding: '1rem', display: 'flex', justifyContent: 'space-between', alignItems: 'center', borderTop: '1px solid var(--border-color)' }}>
                <div style={{ fontSize: '0.875rem', color: 'var(--text-secondary)' }}>
                  Hiển thị {paginatedItems?.length || 0} / {allFilteredAndSortedData?.length || 0} kết quả
                </div>
                <div style={{ display: 'flex', gap: '1rem', alignItems: 'center' }}>
                  <button 
                    className="tncn-btn tncn-btn-secondary" 
                    onClick={() => handlePageChange(currentPage - 1)}
                    disabled={currentPage === 1}
                    style={{ padding: '0.35rem 0.75rem' }}
                  >
                    Trước
                  </button>
                  <span style={{ fontSize: '0.875rem', fontWeight: 500, color: 'var(--text-secondary)' }}>
                    Trang <span style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{currentPage}</span> / <span style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{totalPages}</span>
                  </span>
                  <button 
                    className="tncn-btn tncn-btn-secondary" 
                    onClick={() => handlePageChange(currentPage + 1)}
                    disabled={currentPage === totalPages}
                    style={{ padding: '0.35rem 0.75rem' }}
                  >
                    Sau
                  </button>
                </div>
              </div>

              {showAddForm && (
                <div className="tncn-add-row">
                  <div className="tncn-form-group">
                    <label>Số Bậc</label>
                    <input type="number" className="tncn-form-input" value={addForm.bac || ""} onChange={(e) => setAddForm((p) => ({ ...p, bac: Number(e.target.value) }))} placeholder="VD: 8" />
                  </div>
                  <div className="tncn-form-group">
                    <label>Từ (VNĐ)</label>
                    <input type="number" className="tncn-form-input" value={addForm.tuGia || ""} onChange={(e) => setAddForm((p) => ({ ...p, tuGia: Number(e.target.value) }))} placeholder="VD: 80000000" />
                  </div>
                  <div className="tncn-form-group">
                    <label>Đến (VNĐ)</label>
                    <input type="number" className="tncn-form-input" value={addForm.denGia || ""} onChange={(e) => setAddForm((p) => ({ ...p, denGia: e.target.value ? Number(e.target.value) : null }))} placeholder="Để trống = Không giới hạn" />
                  </div>
                  <div className="tncn-form-group">
                    <label>Thuế suất (%)</label>
                    <input type="number" className="tncn-form-input" value={addForm.thueSuat || ""} onChange={(e) => setAddForm((p) => ({ ...p, thueSuat: Number(e.target.value) }))} placeholder="VD: 35" />
                  </div>
                  <div style={{ display: "flex", gap: "0.5rem", alignItems: "flex-end" }}>
                    <button className="tncn-btn tncn-btn-primary" onClick={submitAddForm} disabled={saving}>
                      {saving ? "Đang lưu..." : "Thêm"}
                    </button>
                    <button className="tncn-btn tncn-btn-outline" onClick={() => setShowAddForm(false)}>Hủy</button>
                  </div>
                </div>
              )}
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
