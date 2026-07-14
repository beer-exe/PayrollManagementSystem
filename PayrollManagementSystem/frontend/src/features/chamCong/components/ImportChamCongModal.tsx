import React, { useState, useRef } from 'react';
import { useChamCong } from '../hooks/useChamCong';
import { chamCongApi } from '../api/chamCongApi';
import type { ImportChamCongResultDto } from '../types/chamCong.types';

interface Props {
  onClose: () => void;
  onSuccess: () => void;
}

export const ImportChamCongModal: React.FC<Props> = ({ onClose, onSuccess }) => {
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [isDragging, setIsDragging] = useState(false);
  const [importing, setImporting] = useState(false);
  const [result, setResult] = useState<ImportChamCongResultDto | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);
  const { importChamCong } = useChamCong();

  const handleFile = (file: File | null) => {
    if (!file) return;
    if (!file.name.endsWith('.csv')) {
      alert('Chỉ hỗ trợ file .csv');
      return;
    }
    setSelectedFile(file);
    setResult(null);
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragging(false);
    handleFile(e.dataTransfer.files[0]);
  };

  const handleImport = async () => {
    if (!selectedFile) return;
    setImporting(true);
    const res = await importChamCong(selectedFile);
    setImporting(false);
    if (res) {
      setResult(res);
      if (res.thanhCong > 0) onSuccess();
    }
  };

  return (
    <div className="cc-modal-overlay" onClick={e => e.target === e.currentTarget && onClose()}>
      <div className="cc-modal" style={{ maxWidth: 560 }}>
        <div className="cc-modal-header">
          <span className="cc-modal-title">📤 Import Chấm Công từ CSV</span>
          <button className="cc-modal-close" onClick={onClose}>×</button>
        </div>

        <div className="cc-modal-body">
          {/* Format guide */}
          <div style={{ background: '#f0fdf4', border: '1px solid #bbf7d0', borderRadius: 8, padding: '12px 16px', marginBottom: 18, fontSize: 13 }}>
            <strong style={{ color: '#166534' }}>📋 Định dạng CSV (có header dòng đầu):</strong>
            <pre style={{ margin: '8px 0 0', color: '#15803d', fontSize: 12, overflow: 'auto' }}>
{`CCCD,NgayChamCong,GioVao,GioRa,GhiChu
001234567890,15/07/2026,08:00,17:00,
001234567891,15/07/2026,08:30,17:00,Đi trễ`}
            </pre>
            <button
              style={{ marginTop: 8, background: 'none', border: '1px solid #166534', color: '#166534', borderRadius: 6, padding: '4px 12px', fontSize: 12, cursor: 'pointer' }}
              onClick={() => chamCongApi.downloadTemplate()}
            >
              ⬇️ Tải file mẫu
            </button>
          </div>

          {/* Drop zone */}
          <div
            className={`cc-dropzone ${isDragging ? 'cc-dropzone--active' : ''} ${selectedFile ? 'cc-dropzone--selected' : ''}`}
            onDragOver={e => { e.preventDefault(); setIsDragging(true); }}
            onDragLeave={() => setIsDragging(false)}
            onDrop={handleDrop}
            onClick={() => fileInputRef.current?.click()}
          >
            <input
              ref={fileInputRef}
              type="file"
              accept=".csv"
              style={{ display: 'none' }}
              onChange={e => handleFile(e.target.files?.[0] ?? null)}
            />
            {selectedFile ? (
              <div className="cc-dropzone__selected">
                <span>📄</span>
                <strong>{selectedFile.name}</strong>
                <span style={{ color: '#6b7280', fontSize: 12 }}>
                  {(selectedFile.size / 1024).toFixed(1)} KB
                </span>
                <button
                  style={{ fontSize: 12, color: '#dc2626', background: 'none', border: 'none', cursor: 'pointer' }}
                  onClick={e => { e.stopPropagation(); setSelectedFile(null); setResult(null); }}
                >
                  Xóa file
                </button>
              </div>
            ) : (
              <div className="cc-dropzone__placeholder">
                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" width={36} height={36} style={{ color: '#a78bfa' }}>
                  <path fillRule="evenodd" d="M1.5 6a2.25 2.25 0 0 1 2.25-2.25h16.5A2.25 2.25 0 0 1 22.5 6v12a2.25 2.25 0 0 1-2.25 2.25H3.75A2.25 2.25 0 0 1 1.5 18V6ZM3 16.06V18c0 .414.336.75.75.75h16.5A.75.75 0 0 0 21 18v-1.94l-2.69-2.689a1.5 1.5 0 0 0-2.12 0l-.88.879.97.97a.75.75 0 1 1-1.06 1.06l-5.16-5.159a1.5 1.5 0 0 0-2.12 0L3 16.061Zm10.125-7.81a1.125 1.125 0 1 1 2.25 0 1.125 1.125 0 0 1-2.25 0Z" clipRule="evenodd" />
                </svg>
                <p style={{ color: '#6b7280', margin: '8px 0 4px' }}>Kéo thả file CSV vào đây hoặc <span style={{ color: '#7c3aed', fontWeight: 600 }}>click để chọn</span></p>
                <p style={{ color: '#9ca3af', fontSize: 12, margin: 0 }}>Chỉ hỗ trợ .csv</p>
              </div>
            )}
          </div>

          {/* Import Result */}
          {result && (
            <div className="cc-import-result">
              <div className="cc-import-stats">
                <div className="cc-import-stat cc-import-stat--total">
                  <span className="cc-import-stat__num">{result.tongSoDong}</span>
                  <span>Tổng dòng</span>
                </div>
                <div className="cc-import-stat cc-import-stat--success">
                  <span className="cc-import-stat__num">{result.thanhCong}</span>
                  <span>Thành công</span>
                </div>
                <div className="cc-import-stat cc-import-stat--fail">
                  <span className="cc-import-stat__num">{result.thatBai}</span>
                  <span>Thất bại</span>
                </div>
              </div>
              {result.loiNhap.length > 0 && (
                <div className="cc-import-errors">
                  <strong style={{ color: '#991b1b', fontSize: 13 }}>⚠️ Chi tiết lỗi:</strong>
                  <ul>
                    {result.loiNhap.map((err, i) => (
                      <li key={i}>{err}</li>
                    ))}
                  </ul>
                </div>
              )}
            </div>
          )}
        </div>

        <div className="cc-modal-footer">
          <button className="cc-btn cc-btn--outline" onClick={onClose}>
            {result ? 'Đóng' : 'Hủy'}
          </button>
          {!result && (
            <button
              className="cc-btn cc-btn--primary"
              onClick={handleImport}
              disabled={!selectedFile || importing}
            >
              {importing ? '⏳ Đang import...' : '📤 Bắt đầu Import'}
            </button>
          )}
        </div>
      </div>

      <style>{`
        .cc-dropzone {
          border: 2px dashed #c4b5fd;
          border-radius: 12px;
          padding: 32px 20px;
          text-align: center;
          cursor: pointer;
          transition: all 0.2s;
          background: #faf5ff;
          margin-bottom: 16px;
        }
        .cc-dropzone:hover, .cc-dropzone--active { border-color: #7c3aed; background: #ede9fe; }
        .cc-dropzone--selected { border-color: #16a34a; background: #f0fdf4; border-style: solid; }
        .cc-dropzone__selected { display: flex; flex-direction: column; align-items: center; gap: 6px; }
        .cc-dropzone__placeholder {}
        .cc-import-result { margin-top: 8px; }
        .cc-import-stats { display: flex; gap: 12px; margin-bottom: 12px; }
        .cc-import-stat {
          flex: 1; text-align: center; padding: 12px 8px;
          border-radius: 10px; font-size: 12px; font-weight: 600;
        }
        .cc-import-stat__num { display: block; font-size: 22px; font-weight: 700; margin-bottom: 2px; }
        .cc-import-stat--total  { background: #f1f5f9; color: #475569; }
        .cc-import-stat--success { background: #dcfce7; color: #166534; }
        .cc-import-stat--fail   { background: #fee2e2; color: #7f1d1d; }
        .cc-import-errors {
          background: #fef2f2; border: 1px solid #fecaca;
          border-radius: 8px; padding: 12px 14px;
        }
        .cc-import-errors ul { margin: 8px 0 0; padding-left: 16px; }
        .cc-import-errors li { font-size: 12px; color: #991b1b; margin-bottom: 4px; }
      `}</style>
    </div>
  );
};
