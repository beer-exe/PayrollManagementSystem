import React, { useRef, useEffect } from 'react';
import type { SystemLogDto } from '../types';
import { LOG_LEVEL_COLORS } from '../types';

interface RealtimePanelProps {
  logs: SystemLogDto[];
  connectionStatus: 'disconnected' | 'connecting' | 'connected';
}

export function RealtimePanel({ logs, connectionStatus }: RealtimePanelProps) {
  const feedRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (feedRef.current) {
      feedRef.current.scrollTop = 0;
    }
  }, [logs.length]);

  const formatTime = (d: string) => {
    const dt = new Date(d);
    return `${dt.getHours().toString().padStart(2, '0')}:${dt.getMinutes().toString().padStart(2, '0')}:${dt.getSeconds().toString().padStart(2, '0')}`;
  };

  const statusLabel = connectionStatus === 'connected' ? 'Đang kết nối' : connectionStatus === 'connecting' ? 'Đang kết nối...' : 'Ngắt kết nối';

  return (
    <div className="syslog-realtime-panel">
      <div className="syslog-realtime-header">
        <span className="syslog-realtime-title">
          <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" style={{ width: 16, height: 16 }}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M3.75 13.5l10.5-11.25L12 10.5h8.25L9.75 21.75 12 13.5H3.75z" />
          </svg>
          Live Feed
        </span>
        <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
          <span className="syslog-realtime-count">{logs.length}</span>
          <span style={{ fontSize: '0.7rem', color: connectionStatus === 'connected' ? '#4ade80' : '#f87171', fontWeight: 600 }}>
            {statusLabel}
          </span>
        </div>
      </div>
      <div className="syslog-realtime-feed" ref={feedRef}>
        {logs.length === 0 ? (
          <div className="syslog-realtime-empty">
            Đang chờ log mới...<br />
            <span style={{ fontSize: '0.75rem', marginTop: 4, display: 'block' }}>Thực hiện request tới hệ thống để xem log</span>
          </div>
        ) : (
          logs.map(log => {
            const levelInfo = LOG_LEVEL_COLORS[log.level] ?? { label: log.level, text: '#d1d5db' };
            return (
              <div key={log.id} className={`syslog-realtime-entry ${log.level}`}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                  <span className="syslog-rt-time">{formatTime(log.raiseDate)}</span>
                  <span className="syslog-badge" style={{ background: levelInfo.bg, color: levelInfo.text, fontSize: '0.65rem' }}>
                    {levelInfo.label}
                  </span>
                </div>
                <div className="syslog-rt-msg">{log.message ?? '—'}</div>
              </div>
            );
          })
        )}
      </div>
    </div>
  );
}
