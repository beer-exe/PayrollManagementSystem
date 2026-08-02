import React, { useState, useRef, useEffect } from 'react';
import { type SystemLogDto, LOG_LEVEL_COLORS } from '../types';

interface LogDetailModalProps {
  log: SystemLogDto;
  onClose: () => void;
}

export function LogDetailModal({ log, onClose }: LogDetailModalProps) {
  const overlayRef = useRef<HTMLDivElement>(null);
  const levelInfo = LOG_LEVEL_COLORS[log.level] ?? { bg: '#f3f4f6', text: '#374151', label: log.level };

  useEffect(() => {
    const handleKey = (e: KeyboardEvent) => { if (e.key === 'Escape') onClose(); };
    document.addEventListener('keydown', handleKey);
    return () => document.removeEventListener('keydown', handleKey);
  }, [onClose]);

  const formatDate = (d: string) => new Date(d).toLocaleString('vi-VN', {
    year: 'numeric', month: '2-digit', day: '2-digit',
    hour: '2-digit', minute: '2-digit', second: '2-digit',
  });

  let parsedProperties: string | null = null;
  if (log.properties) {
    try {
      parsedProperties = JSON.stringify(JSON.parse(log.properties), null, 2);
    } catch {
      parsedProperties = log.properties;
    }
  }

  return (
    <div className="syslog-modal-overlay" ref={overlayRef} onClick={e => { if (e.target === overlayRef.current) onClose(); }}>
      <div className="syslog-modal">
        <div className="syslog-modal-header">
          <span className="syslog-modal-title">🔍 Chi tiết Log</span>
          <button className="syslog-modal-close" onClick={onClose}>✕</button>
        </div>
        <div className="syslog-modal-body">
          <div className="syslog-modal-meta">
            <div className="syslog-modal-meta-item">
              <div className="syslog-modal-meta-key">Thời gian</div>
              <div className="syslog-modal-meta-val">{formatDate(log.raiseDate)}</div>
            </div>
            <div className="syslog-modal-meta-item">
              <div className="syslog-modal-meta-key">Level</div>
              <div className="syslog-modal-meta-val">
                <span className="syslog-badge" style={{ background: levelInfo.bg, color: levelInfo.text }}>
                  {levelInfo.label}
                </span>
              </div>
            </div>
          </div>

          {log.message && (
            <div className="syslog-modal-section">
              <div className="syslog-modal-section-label">Message</div>
              <div className="syslog-modal-code">{log.message}</div>
            </div>
          )}

          {log.exception && (
            <div className="syslog-modal-section">
              <div className="syslog-modal-section-label">Exception / Stack Trace</div>
              <div className="syslog-modal-code error">{log.exception}</div>
            </div>
          )}

          {parsedProperties && (
            <div className="syslog-modal-section">
              <div className="syslog-modal-section-label">Properties (JSON)</div>
              <div className="syslog-modal-code">{parsedProperties}</div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
