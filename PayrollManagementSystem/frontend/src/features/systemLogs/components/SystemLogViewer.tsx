import React, { useEffect, useState, useRef } from 'react';
import './SystemLogViewer.css';
import { SortableHeader } from '@/components/DataTable/SortableHeader';
import { ExportButtons } from '@/components/DataTable/ExportButtons';
import { exportToExcel, exportToPdf, ExportColumn } from '@/utils/exportUtils';
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

  const [sortKey, setSortKey] = useState<string>('');
  const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('desc');

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

  const handleSort = (key: string) => {
    let newDirection: 'asc' | 'desc' = 'asc';
    if (sortKey === key) {
      newDirection = sortDirection === 'asc' ? 'desc' : 'asc';
    }
    setSortKey(key);
    setSortDirection(newDirection);
    applyFilter({ sortBy: key, sortDirection: newDirection });
  };

  const handleExportExcel = () => {
    const columns: ExportColumn<SystemLogDto>[] = [
      { header: 'Thời gian', key: 'raiseDate', render: (item) => formatDate(item.raiseDate) },
      { header: 'Level', key: 'level' },
      { header: 'Message', key: 'message' },
      { header: 'Exception', key: 'exception' },
    ];
    exportToExcel(logs, columns, `System_Logs.xlsx`);
  };

  const handleExportPdf = () => {
    const columns: ExportColumn<SystemLogDto>[] = [
      { header: 'Thời gian', key: 'raiseDate', render: (item) => formatDate(item.raiseDate) },
      { header: 'Level', key: 'level' },
      { header: 'Message', key: 'message' }
    ];
    exportToPdf(logs, columns, `System_Logs.pdf`, `DANH SÁCH LOG HỆ THỐNG`);
  };

  const totalPages = pagedResult?.totalPages ?? 1;
  const pageNumber = pagedResult?.pageNumber ?? 1;
  const totalRecords = pagedResult?.totalRecords ?? 0;

  return (
    <div className="syslog-container">
      {/* Header */}
      <div className="syslog-header">
        <div className="syslog-header-title">
          <h2>
            <span className="syslog-header-title-icon">🖥️</span>
            Giám sát Log Hệ thống
          </h2>
        </div>
        <button
          className={`syslog-realtime-toggle ${realtimeEnabled ? 'active' : ''}`}
          onClick={() => setRealtimeEnabled(v => !v)}
        >
          <span className="syslog-toggle-dot" />
          <span className="syslog-toggle-label">{realtimeEnabled ? 'Real-time ON' : 'Real-time'}</span>
          {realtimeEnabled && (
            <span className="syslog-connection-badge">
              {connectionStatus === 'connected' ? '● Live' : connectionStatus === 'connecting' ? '◌ ...' : '○ Off'}
            </span>
          )}
        </button>
      </div>

      <div className={`syslog-content ${realtimeEnabled ? 'with-realtime' : ''}`} ref={menuRef}>
        <div className="syslog-controls-wrapper">
          {/* Filter bar */}
          <div className="syslog-filterbar">
        <div className="syslog-filter-inputs" style={{ display: 'flex', gap: '1rem', flexWrap: 'wrap', flex: 1 }}>
          <div className="syslog-filter-group">
            <label className="syslog-filter-label">Level</label>
            <select className="syslog-filter-select" value={localLevel} onChange={e => setLocalLevel(e.target.value)}>
              <option value="">Tất cả</option>
              {LOG_LEVELS.map(l => <option key={l} value={l}>{l}</option>)}
            </select>
          </div>
          <div className="syslog-filter-group">
            <label className="syslog-filter-label">Từ ngày</label>
            <input type="date" className="syslog-filter-input" style={{ minWidth: 150 }} value={localFromDate} onChange={e => setLocalFromDate(e.target.value)} />
          </div>
          <div className="syslog-filter-group">
            <label className="syslog-filter-label">Đến ngày</label>
            <input type="date" className="syslog-filter-input" style={{ minWidth: 150 }} value={localToDate} onChange={e => setLocalToDate(e.target.value)} />
          </div>
          <div className="syslog-filter-group" style={{ flex: 1, minWidth: '200px' }}>
            <label className="syslog-filter-label">Tìm kiếm</label>
            <input
              type="text"
              className="syslog-filter-input"
              style={{ width: '100%' }}
              placeholder="Nhập từ khóa trong message..."
              value={localKeyword}
              onChange={e => setLocalKeyword(e.target.value)}
              onKeyDown={e => { if (e.key === 'Enter') handleSearch(); }}
            />
          </div>
        </div>
        <div className="syslog-filter-actions" style={{ width: '100%', justifyContent: 'flex-end' }}>
          <button className="syslog-btn syslog-btn-primary" onClick={handleSearch}>🔍 Tìm kiếm</button>
          <button className="syslog-btn syslog-btn-secondary" onClick={handleReset}>↺ Đặt lại</button>
          <button className="syslog-btn syslog-btn-secondary" onClick={() => fetchLogs()}>⟳ Làm mới</button>
          <ExportButtons onExportExcel={handleExportExcel} onExportPdf={handleExportPdf} />
        </div>
          </div>

          {/* Error */}
          {error && <div className="syslog-error-banner" style={{ margin: '1rem' }}>⚠️ {error}</div>}

          {/* Log table */}
          <div className="syslog-table-container custom-scrollbar">
              <table className="syslog-table">
              <thead>
                <tr>
                  <SortableHeader label="Thời gian" sortKey="raiseDate" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ width: '155px' }} />
                  <SortableHeader label="Level" sortKey="level" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ width: '70px' }} />
                  <SortableHeader label="Message" sortKey="message" currentSortKey={sortKey} currentSortDirection={sortDirection} onSort={handleSort} style={{ maxWidth: '600px' }} />
                  <th className="syslog-col-action"></th>
                </tr>
              </thead>
              <tbody>
                {loading ? (
                  <tr className="syslog-loading-row">
                    <td colSpan={4}><span className="syslog-spinner" />Đang tải...</td>
                  </tr>
                ) : logs.length === 0 ? (
                  <tr>
                    <td colSpan={4}>
                      <div className="syslog-empty">
                        <div className="syslog-empty-icon">📋</div>
                        <div>Không có log nào phù hợp</div>
                      </div>
                    </td>
                  </tr>
                ) : (
                  logs.map(log => {
                    const levelInfo = LOG_LEVEL_COLORS[log.level] ?? { bg: '#f3f4f6', text: '#374151', label: log.level };
                    return (
                      <tr key={log.id}>
                        <td className="syslog-col-time">{formatDate(log.raiseDate)}</td>
                        <td className="syslog-col-level">
                          <span className="syslog-badge" style={{ background: levelInfo.bg, color: levelInfo.text }}>
                            {levelInfo.label}
                          </span>
                        </td>
                        <td className="syslog-col-msg">
                          <div className="syslog-msg-text">{log.message ?? '—'}</div>
                        </td>
                        <td className="syslog-col-action">
                          <div className="syslog-action-menu-wrapper">
                            <button
                              className="syslog-action-btn"
                              onClick={() => setOpenMenuId(openMenuId === log.id ? null : log.id)}
                            >···</button>
                            {openMenuId === log.id && (
                              <div className="syslog-dropdown">
                                <div className="syslog-dropdown-item" onClick={() => { setSelectedLog(log); setOpenMenuId(null); }}>
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
            <div className="syslog-pagination" style={{ justifyContent: 'flex-end', background: 'var(--bg-body)' }}>
              <div className="syslog-page-controls" style={{ display: 'flex', gap: '1rem', alignItems: 'center' }}>
                <button 
                  className="syslog-btn syslog-btn-secondary" 
                  onClick={() => changePage(pageNumber - 1)} 
                  disabled={pageNumber <= 1}
                  style={{ padding: '0.35rem 0.75rem' }}
                >
                  Trước
                </button>
                <span className="syslog-page-indicator" style={{ fontSize: '0.875rem', fontWeight: 500, color: 'var(--text-secondary)' }}>
                  Trang <span style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{pageNumber}</span> / <span style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{totalPages}</span>
                </span>
                <button 
                  className="syslog-btn syslog-btn-secondary" 
                  onClick={() => changePage(pageNumber + 1)} 
                  disabled={pageNumber >= totalPages}
                  style={{ padding: '0.35rem 0.75rem' }}
                >
                  Sau
                </button>
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
