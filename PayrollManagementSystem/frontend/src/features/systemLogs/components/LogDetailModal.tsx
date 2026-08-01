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
    <div className="sl-modal-overlay" ref={overlayRef} onClick={e => { if (e.target === overlayRef.current) onClose(); }}>
      <div className="sl-modal">
        <div className="sl-modal-header">
          <span className="sl-modal-title">🔍 Chi tiết Log</span>
          <button className="sl-modal-close" onClick={onClose}>✕</button>
        </div>
        <div className="sl-modal-body">
          <div className="sl-modal-meta">
            <div className="sl-modal-meta-item">
              <div className="sl-modal-meta-key">Thời gian</div>
              <div className="sl-modal-meta-val">{formatDate(log.raiseDate)}</div>
            </div>
            <div className="sl-modal-meta-item">
              <div className="sl-modal-meta-key">Level</div>
              <div className="sl-modal-meta-val">
                <span className="sl-badge" style={{ background: levelInfo.bg, color: levelInfo.text }}>
                  {levelInfo.label}
                </span>
              </div>
            </div>
          </div>

          {log.message && (
            <div className="sl-modal-section">
              <div className="sl-modal-section-label">Message</div>
              <div className="sl-modal-code">{log.message}</div>
            </div>
          )}

          {log.exception && (
            <div className="sl-modal-section">
              <div className="sl-modal-section-label">Exception / Stack Trace</div>
              <div className="sl-modal-code error">{log.exception}</div>
            </div>
          )}

          {parsedProperties && (
            <div className="sl-modal-section">
              <div className="sl-modal-section-label">Properties (JSON)</div>
              <div className="sl-modal-code">{parsedProperties}</div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
