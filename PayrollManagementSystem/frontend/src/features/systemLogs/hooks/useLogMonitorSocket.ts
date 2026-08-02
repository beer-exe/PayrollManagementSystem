import { useState, useEffect, useRef, useCallback } from 'react';
import type { HubConnection } from '@microsoft/signalr';
import { HubConnectionState } from '@microsoft/signalr';
import type { SystemLogDto } from '../types';
import { createLogMonitorConnection, mapRealtimeToDto, type RealtimeLogEvent } from '../api/logMonitorSocket';

const MAX_REALTIME_LOGS = 200;

export function useLogMonitorSocket(enabled: boolean) {
  const [realtimeLogs, setRealtimeLogs] = useState<SystemLogDto[]>([]);
  const [connectionStatus, setConnectionStatus] = useState<'disconnected' | 'connecting' | 'connected'>('disconnected');
  const connectionRef = useRef<HubConnection | null>(null);
  const counterRef = useRef(0);

  const connect = useCallback(async () => {
    const token = localStorage.getItem('accessToken');
    if (!token) return;

    setConnectionStatus('connecting');
    const connection = createLogMonitorConnection(token);
    connectionRef.current = connection;

    connection.on('ReceiveLog', (event: RealtimeLogEvent) => {
      counterRef.current -= 1;
      const dto = mapRealtimeToDto(event, counterRef.current);
      setRealtimeLogs(prev => {
        const updated = [dto, ...prev];
        return updated.length > MAX_REALTIME_LOGS ? updated.slice(0, MAX_REALTIME_LOGS) : updated;
      });
    });

    connection.onreconnecting(() => setConnectionStatus('connecting'));
    connection.onreconnected(() => setConnectionStatus('connected'));
    connection.onclose(() => setConnectionStatus('disconnected'));

    try {
      await connection.start();
      setConnectionStatus('connected');
    } catch {
      setConnectionStatus('disconnected');
    }
  }, []);

  const disconnect = useCallback(async () => {
    if (connectionRef.current && connectionRef.current.state !== HubConnectionState.Disconnected) {
      await connectionRef.current.stop();
    }
    connectionRef.current = null;
    setConnectionStatus('disconnected');
    setRealtimeLogs([]);
  }, []);

  useEffect(() => {
    if (enabled) {
      connect();
    } else {
      disconnect();
    }
    return () => {
      disconnect();
    };
  }, [enabled]);

  return { realtimeLogs, connectionStatus };
}
