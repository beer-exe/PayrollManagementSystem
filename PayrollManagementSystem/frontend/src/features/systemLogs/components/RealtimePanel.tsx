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
    <div className="sl-realtime-panel">
      <div className="sl-realtime-header">
        <span className="sl-realtime-title">⚡ Live Feed</span>
        <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
          <span className="sl-realtime-count">{logs.length}</span>
          <span style={{ fontSize: '0.7rem', color: connectionStatus === 'connected' ? '#4ade80' : '#f87171', fontWeight: 600 }}>
            {statusLabel}
          </span>
        </div>
      </div>
      <div className="sl-realtime-feed" ref={feedRef}>
        {logs.length === 0 ? (
          <div className="sl-realtime-empty">
            Đang chờ log mới...<br />
            <span style={{ fontSize: '0.75rem', marginTop: 4, display: 'block' }}>Thực hiện request tới hệ thống để xem log</span>
          </div>
        ) : (
          logs.map(log => {
            const levelInfo = LOG_LEVEL_COLORS[log.level] ?? { label: log.level, text: '#d1d5db' };
            return (
              <div key={log.id} className={`sl-realtime-entry ${log.level}`}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                  <span className="sl-rt-time">{formatTime(log.raiseDate)}</span>
                  <span className="sl-badge" style={{ background: 'rgba(255,255,255,0.1)', color: levelInfo.text, fontSize: '0.65rem' }}>
                    {levelInfo.label}
                  </span>
                </div>
                <div className="sl-rt-msg">{log.message ?? '—'}</div>
              </div>
            );
          })
        )}
      </div>
    </div>
  );
}
