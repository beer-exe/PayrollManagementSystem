import React, { useState, useRef, useEffect } from 'react';
import './ChamCongManagement.css';

interface Props {
  value: string;
  onChange: (val: string) => void;
  placeholder?: string;
  error?: boolean;
}

export const ClockTimePicker: React.FC<Props> = ({ value, onChange, placeholder, error }) => {
  const [isOpen, setIsOpen] = useState(false);
  const [mode, setMode] = useState<'hour' | 'minute'>('hour');
  const [isDragging, setIsDragging] = useState(false);
  
  const h = value ? parseInt(value.split(':')[0]) || 0 : 0;
  const m = value ? parseInt(value.split(':')[1]) || 0 : 0;

  const clockRef = useRef<HTMLDivElement>(null);
  const popoverRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (popoverRef.current && !popoverRef.current.contains(e.target as Node)) {
        setIsOpen(false);
      }
    };
    if (isOpen) {
      document.addEventListener('mousedown', handleClickOutside);
    }
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, [isOpen]);

  const updateTime = (clientX: number, clientY: number, complete: boolean = false) => {
    if (!clockRef.current) return;
    const rect = clockRef.current.getBoundingClientRect();
    const cx = rect.width / 2;
    const cy = rect.height / 2;
    const x = clientX - rect.left;
    const y = clientY - rect.top;

    let angle = (Math.atan2(y - cy, x - cx) * 180) / Math.PI + 90;
    if (angle < 0) angle += 360;

    const dist = Math.sqrt((x - cx) ** 2 + (y - cy) ** 2);

    if (mode === 'hour') {
      let calcH = Math.round(angle / 30) % 12;
      const isInner = dist < rect.width * 0.35;
      if (calcH === 0) calcH = 12; 

      if (isInner) {
        if (calcH === 12) calcH = 0;
        else calcH += 12;
      } else {
        if (calcH === 12) calcH = 12; 
      }
      
      onChange(`${calcH.toString().padStart(2, '0')}:${m.toString().padStart(2, '0')}`);
      if (complete) setMode('minute');
    } else {
      let calcM = Math.round(angle / 6) % 60;
      onChange(`${h.toString().padStart(2, '0')}:${calcM.toString().padStart(2, '0')}`);
      if (complete) setIsOpen(false);
    }
  };

  const handlePointerDown = (e: React.PointerEvent) => {
    setIsDragging(true);
    updateTime(e.clientX, e.clientY, false);
    (e.target as HTMLElement).setPointerCapture(e.pointerId);
  };
  
  const handlePointerMove = (e: React.PointerEvent) => {
    if (isDragging) updateTime(e.clientX, e.clientY, false);
  };
  
  const handlePointerUp = (e: React.PointerEvent) => {
    if (isDragging) {
      setIsDragging(false);
      updateTime(e.clientX, e.clientY, true);
      (e.target as HTMLElement).releasePointerCapture(e.pointerId);
    }
  };

  const renderNumbers = () => {
    if (mode === 'hour') {
      const outer = [12, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];
      const inner = [0, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23];
      return (
        <>
          {outer.map((num, i) => {
            const a = (i * 30 - 90) * (Math.PI / 180);
            const x = 50 + 40 * Math.cos(a); 
            const y = 50 + 40 * Math.sin(a);
            const active = h === num;
            return (
              <div key={`o-${num}`} className={`cc-clock-num ${active ? 'active' : ''}`} style={{ left: `${x}%`, top: `${y}%` }}>
                {num}
              </div>
            );
          })}
          {inner.map((num, i) => {
            const a = (i * 30 - 90) * (Math.PI / 180);
            const x = 50 + 26 * Math.cos(a); 
            const y = 50 + 26 * Math.sin(a);
            const active = h === num;
            return (
              <div key={`i-${num}`} className={`cc-clock-num inner ${active ? 'active' : ''}`} style={{ left: `${x}%`, top: `${y}%` }}>
                {num === 0 ? '00' : num}
              </div>
            );
          })}
        </>
      );
    } else {
      const mins = [0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55];
      return mins.map((num, i) => {
        const a = (i * 30 - 90) * (Math.PI / 180);
        const x = 50 + 40 * Math.cos(a);
        const y = 50 + 40 * Math.sin(a);
        const active = m === num || (!isDragging && m % 5 !== 0 && Math.abs(m - num) < 3); // approximate active for display if picked a non-5 min
        return (
          <div key={`m-${num}`} className={`cc-clock-num ${active ? 'active' : ''}`} style={{ left: `${x}%`, top: `${y}%` }}>
            {num.toString().padStart(2, '0')}
          </div>
        );
      });
    }
  };

  const getHandStyle = () => {
    let angle = 0;
    let length = '40%';
    if (mode === 'hour') {
      angle = (h % 12) * 30;
      if (h === 0 || h > 12) length = '26%';
    } else {
      angle = m * 6;
    }
    return {
      transform: `rotate(${angle}deg)`,
      height: length,
      transition: isDragging ? 'none' : 'transform 0.2s cubic-bezier(0.4, 0, 0.2, 1), height 0.2s'
    };
  };

  return (
    <div className="cc-clock-picker" ref={popoverRef}>
      <input
        type="text"
        readOnly
        className={`cc-form-control cc-clock-input ${error ? 'error' : ''}`}
        placeholder={placeholder || "--:--"}
        value={value}
        onClick={() => {
          setIsOpen(!isOpen);
          setMode('hour');
        }}
      />
      
      {isOpen && (
        <div className="cc-clock-popover">
          <div className="cc-clock-header">
            <span 
              className={mode === 'hour' ? 'active' : ''} 
              onClick={() => setMode('hour')}
            >
              {value ? h.toString().padStart(2, '0') : '--'}
            </span>
            <span style={{ opacity: 0.5, cursor: 'default' }}>:</span>
            <span 
              className={mode === 'minute' ? 'active' : ''} 
              onClick={() => setMode('minute')}
            >
              {value ? m.toString().padStart(2, '0') : '--'}
            </span>
          </div>
          
          <div className="cc-clock-body">
            <div 
              className="cc-clock-face" 
              ref={clockRef}
              onPointerDown={handlePointerDown}
              onPointerMove={handlePointerMove}
              onPointerUp={handlePointerUp}
              onPointerCancel={handlePointerUp}
            >
              <div className="cc-clock-center" />
              <div className="cc-clock-hand" style={getHandStyle()} />
              {renderNumbers()}
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
