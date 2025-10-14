import http from 'k6/http';
import { check, sleep } from 'k6';

import { getRandom10DigitNumber, getRandomName } from '../utils/utils.js'

export const options = {
    //vus: 1,
    //duration: '10s',
    thresholds: {
        // We want 95% of our requests to take less than 300ms
        http_req_duration: ['p(95)<300'],
        // We want at most 10% of our requests to fail
        http_req_failed:['rate<0.1']
    },
    stages: [
        // Load testing
        // Gradually increase the load to 100 users
        {duration:"10s", target: 10},
        // Maintain a high load
        {duration:"10s", target: 10},
        // Ramp down really fast
        {duration:"3s", target: 0},

        // // Approach 2 : Stress testing
        // // Right below the normal usage of the application
        // {duration: "10s", target: 100},
        // {duration: "15s", target: 100},
        // {duration: "10s", target: 200},
        // {duration: "15s", target: 200},
        // // Maximum theoretical number of users we want to support:
        // {duration: "10s", target: 300},
        // // Stay there for an extended period of time
        // {duration: "15s", target: 300},
        // // Ramp down
        // {duration: "5s", target: 0}


        // // Approach 3: Spike test
        // {duration: "10s", target: 10},
        // // Suddenly ramp up the load to over the theoretical threshold
        // {duration: "5s", target: 100},
        // // Stay there for a while
        // {duration: "10s", target: 100},
        // // Ramp down to an average load
        // {duration: "5s", target: 10},
        // // Stay there to evaluate how the system recovers
        // {duration: "20s", target: 10},
        // // Ramp down completely
        // {duration: "5s", target: 0}


        // Levels of handling:
        // 1 - everything good, no impact on users
        // 2 - overall good, but maybe some small hic'ups here and there (performance issues and such)
        // 3 - users had degraged performance, impacted
        // 4 - death (collapsed under load)
    ]
};

const BASE_API_URL = 'http://localhost:5210';


export default () => {

    const params = {
        headers: {
            'Content-Type' : 'application/json'
        }
    };

    // You can use the dot-env js package to read the values from environment or .env files
    const authPayload = JSON.stringify({
        email : 'jhon.doe@example.com',
        password : 'Passw0rd!'
    });

    const loginResponse = http.post(`${BASE_API_URL}/login`, authPayload, params);

    check(loginResponse, {
        'Get auth token': (response) => response.status === 200 && response.json('accessToken') !== ''
    });

    const authHeaders = {
        headers: {
            Authorization: `Bearer ${loginResponse.json('accessToken')}`,
            'Content-Type' : 'application/json'
        }
    };

    const bookAuthor = getRandomName();
    const bookTitle = `Adventures of ${bookAuthor}`;
    const bookIsbn = `${getRandom10DigitNumber()}`;

    const payload = JSON.stringify({
        title: bookTitle,
        author: bookAuthor,
        isbn: bookIsbn
    });

    const books = http.post(`${BASE_API_URL}/api/v1/books`,payload, authHeaders);

    check(books, {
        "Request is successful": (response) => response.status === 201,
        "The book author is correct": (response) => response.json('author') === bookAuthor,
        "The book title is correct": (response) => response.json('title') === bookTitle,
        "The book isbn is correct": (response) => response.json('isbn') === bookIsbn,
    });

    sleep(1);
}