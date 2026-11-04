import http from 'k6/http';
import { check, sleep } from 'k6';

// Test Configuration: 5 concurrent virtual users (VUs) for 30 seconds
export const options = {
    vus: 5,            // 5 concurrent users
    duration: '30s',   // Test duration
    thresholds: {
        http_req_duration: ['p(95)<500'], // 95% of requests should be below 500ms
        http_req_failed: ['rate<0.01'],   // Error rate should be less than 1%
    },
};

export default function () {
    // Replace with your local web app URL (e.g., Home or Donate page)
    const res = http.get('https://localhost:7006/');

    // Validate that the server responds successfully
    check(res, {
        'status is 200': (r) => r.status === 200,
        'response time is acceptable': (r) => r.timings.duration < 500,
    });

    sleep(1); // Wait 1 second between requests per virtual user
}