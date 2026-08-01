import React, { useEffect, useState, useRef } from 'react';
import './SystemLogViewer.css';
import type { SystemLogDto } from '../types';
import { LOG_LEVELS, LOG_LEVEL_COLORS } from '../types';
import { useSystemLogs } from '../hooks/useSystemLogs';
import { useLogMonitorSocket } from '../hooks/useLogMonitorSocket';
import { LogDetailModal } from './LogDetailModal';
import { RealtimePanel } from './RealtimePanel';

export function SystemLogViewer() {
  const { logs, pagedResult, loading, error, fetchLogs, applyFilter, changePage, resetFilter } = useSystemLogs();
  const [realtimeEnabled, setRealtimeEnabled] = useState(false);
  const { realtimeLogs, connectionStatus } = useLogMonitorSocket(realtimeEnabled);
  const [selectedLog, setSelectedLog] = useState<SystemLogDto | null>(null);
  const [openMenuId, setOpenMenuId] = useState<number | null>(null);

  // Local filter state (uncommitted until user searches)
  const [localKeyword, setLocalKeyword] = useState('');
  const [localLevel, setLocalLevel] = useState('');
  const [localFromDate, setLocalFromDate] = useState('');
  const [localToDate, setLocalToDate] = useState('');

  const menuRef = useRef<HTMLDivElement>(null);

  useEffect(() => { fetchLogs(); }, []);

  useEffect(() => {
    const handleClick = (e: MouseEvent) => {
      if (menuRef.current && !menuRef.current.contains(e.target as Node)) {
        setOpenMenuId(null);
      }
    };
    document.addEventListener('mousedown', handleClick);
    return () => document.removeEventListener('mousedown', handleClick);
  }, []);

  const handleSearch = () => {
    applyFilter({
      level: localLevel || undefined,
      fromDate: localFromDate || undefined,
      toDate: localToDate || undefined,
      keyword: localKeyword || undefined,
    });
  };

  const handleReset = () => {
    setLocalKeyword('');
    setLocalLevel('');
    setLocalFromDate('');
    setLocalToDate('');
    resetFilter();
  };

  const formatDate = (d: string) => {
    const dt = new Date(d);
    return `${dt.getDate().toString().padStart(2,'0')}/${(dt.getMonth()+1).toString().padStart(2,'0')} ${dt.getHours().toString().padStart(2,'0')}:${dt.getMinutes().toString().padStart(2,'0')}:${dt.getSeconds().toString().padStart(2,'0')}`;
  };

  const totalPages = pagedResult?.totalPages ?? 1;
  const pageNumber = pagedResult?.pageNumber ?? 1;
  const totalRecords = pagedResult?.totalRecords ?? 0;

  return (
    <div className="sl-page">
      {/* Header */}
      <div className="sl-header">
        <h1 className="sl-title">
          <span className="sl-title-icon">🖥️</span>
          Giám sát Log Hệ thống
        </h1>
        <button
          className={`sl-realtime-toggle ${realtimeEnabled ? 'active' : ''}`}
          onClick={() => setRealtimeEnabled(v => !v)}
        >
          <span className="sl-toggle-dot" />
          <span className="sl-toggle-label">{realtimeEnabled ? 'Real-time ON' : 'Real-time'}</span>
          {realtimeEnabled && (
            <span className="sl-connection-badge">
              {connectionStatus === 'connected' ? '● Live' : connectionStatus === 'connecting' ? '◌ ...' : '○ Off'}
            </span>
          )}
        </button>
      </div>

      {/* Filter bar */}
      <div className="sl-filterbar">
        <div className="sl-filter-group">
          <label className="sl-filter-label">Level</label>
          <select className="sl-filter-select" value={localLevel} onChange={e => setLocalLevel(e.target.value)}>
            <option value="">Tất cả</option>
            {LOG_LEVELS.map(l => <option key={l} value={l}>{l}</option>)}
          </select>
        </div>
        <div className="sl-filter-group">
          <label className="sl-filter-label">Từ ngày</label>
          <input type="date" className="sl-filter-input" style={{ minWidth: 150 }} value={localFromDate} onChange={e => setLocalFromDate(e.target.value)} />
        </div>
        <div className="sl-filter-group">
          <label className="sl-filter-label">Đến ngày</label>
          <input type="date" className="sl-filter-input" style={{ minWidth: 150 }} value={localToDate} onChange={e => setLocalToDate(e.target.value)} />
        </div>
        <div className="sl-filter-group">
          <label className="sl-filter-label">Tìm kiếm</label>
          <input
            type="text"
            className="sl-filter-input"
            placeholder="Nhập từ khóa trong message..."
            value={localKeyword}
            onChange={e => setLocalKeyword(e.target.value)}
            onKeyDown={e => { if (e.key === 'Enter') handleSearch(); }}
          />
        </div>
        <div className="sl-filter-actions">
          <button className="sl-btn sl-btn-primary" onClick={handleSearch}>🔍 Tìm kiếm</button>
          <button className="sl-btn sl-btn-ghost" onClick={handleReset}>↺ Đặt lại</button>
          <button className="sl-btn sl-btn-ghost" onClick={() => fetchLogs()}>⟳ Làm mới</button>
        </div>
      </div>

      {/* Error */}
      {error && <div className="sl-error-banner">⚠️ {error}</div>}

      {/* Main content */}
      <div className={`sl-content ${realtimeEnabled ? 'with-realtime' : ''}`} ref={menuRef}>
        {/* Log table */}
        <div className="sl-card">
          <div className="sl-table-wrapper">
            <table className="sl-table">
              <thead>
                <tr>
                  <th className="sl-col-time">Thời gian</th>
                  <th className="sl-col-level">Level</th>
                  <th className="sl-col-msg">Message</th>
                  <th className="sl-col-action"></th>
                </tr>
              </thead>
              <tbody>
                {loading ? (
                  <tr className="sl-loading-row">
                    <td colSpan={4}><span className="sl-spinner" />Đang tải...</td>
                  </tr>
                ) : logs.length === 0 ? (
                  <tr>
                    <td colSpan={4}>
                      <div className="sl-empty">
                        <div className="sl-empty-icon">📋</div>
                        <div>Không có log nào phù hợp</div>
                      </div>
                    </td>
                  </tr>
                ) : (
                  logs.map(log => {
                    const levelInfo = LOG_LEVEL_COLORS[log.level] ?? { bg: '#f3f4f6', text: '#374151', label: log.level };
                    return (
                      <tr key={log.id}>
                        <td className="sl-col-time">{formatDate(log.raiseDate)}</td>
                        <td className="sl-col-level">
                          <span className="sl-badge" style={{ background: levelInfo.bg, color: levelInfo.text }}>
                            {levelInfo.label}
                          </span>
                        </td>
                        <td className="sl-col-msg">
                          <div className="sl-msg-text">{log.message ?? '—'}</div>
                        </td>
                        <td className="sl-col-action">
                          <div className="sl-action-menu-wrapper">
                            <button
                              className="sl-action-btn"
                              onClick={() => setOpenMenuId(openMenuId === log.id ? null : log.id)}
                            >···</button>
                            {openMenuId === log.id && (
                              <div className="sl-dropdown">
                                <div className="sl-dropdown-item" onClick={() => { setSelectedLog(log); setOpenMenuId(null); }}>
                                  🔍 Xem chi tiết
                                </div>
                              </div>
                            )}
                          </div>
                        </td>
                      </tr>
                    );
                  })
                )}
              </tbody>
            </table>
          </div>

          {/* Pagination */}
          {!loading && totalRecords > 0 && (
            <div className="sl-pagination">
              <span className="sl-page-info">Tổng: {totalRecords.toLocaleString('vi-VN')} bản ghi</span>
              <div className="sl-page-controls">
                <button className="sl-page-btn" onClick={() => changePage(pageNumber - 1)} disabled={pageNumber <= 1}>‹ Trước</button>
                <span className="sl-page-indicator">{pageNumber}/{totalPages}</span>
                <button className="sl-page-btn" onClick={() => changePage(pageNumber + 1)} disabled={pageNumber >= totalPages}>Sau ›</button>
              </div>
            </div>
          )}
        </div>

        {/* Real-time panel */}
        {realtimeEnabled && (
          <RealtimePanel logs={realtimeLogs} connectionStatus={connectionStatus} />
        )}
      </div>

      {/* Modal */}
      {selectedLog && <LogDetailModal log={selectedLog} onClose={() => setSelectedLog(null)} />}
    </div>
  );
}
