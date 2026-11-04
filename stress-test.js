import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    stages: [
        { duration: '10s', target: 10 },
        { duration: '20s', target: 30 },
        { duration: '10s', target: 50 },
        { duration: '10s', target: 0 },
    ],
    thresholds: {
        http_req_failed: ['rate<0.05'],
    },
};

export default function () {
    const res = http.get('https://localhost:7006/');
    check(res, {
        'status is 200 or 500 handled': (r) => r.status === 200 || r.status === 500,
    });
    sleep(0.5);
}