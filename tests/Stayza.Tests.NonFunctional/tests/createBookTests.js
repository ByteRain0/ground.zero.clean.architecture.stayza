import http from 'k6/http';
import {check, sleep} from 'k6';
import { getRandom10DigitNumber, getRandomName } from '../utils.js';

export const options = {
    stages: [
        {duration:"5s", target: 10},
        {duration:"10s", target: 50000},
        {duration:"5s", target: 0},
    ],
    thresholds: {
        // We want 95% of our requests to take less than 300ms
        http_req_duration: ['p(95)<300'],
        // We want at most 10% of our requests to fail
        http_req_failed:['rate<0.1']
    }
}

const BASE_API_URL = 'http://localhost:5210';

export default () => {

    // Arrange
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

    check(loginResponse, {
        'Get auth token' : (response) => response.status === 200 && response.json('accessToken') !== ''
    });

    const authHeader = {
        headers: {
            'Authorization' : `Bearer ${loginResponse.json('accessToken')}`,
            'Content-Type': 'application/json'
        }
    }

    // Act
    const bookAuthor = getRandomName();
    const bookTitle = `Adventures of ${bookAuthor}`;
    const bookIsbn = `${getRandom10DigitNumber()}`;

    const payload = JSON.stringify({
        title: bookTitle,
        author: bookAuthor,
        isbn: bookIsbn
    });

    // Post the request
    const book = http.post(`${BASE_API_URL}/api/v1/books`, payload, authHeader);

    // Assert
    check(book, {
        "Request is successful": (response) => response.status === 201,
        "The book author is correct": (response) => response.json('author') === bookAuthor,
        "The book title is correct": (response) => response.json('title') === bookTitle,
        "The book isbn is correct": (response) => response.json('isbn') === bookIsbn,
    });

    sleep(1);
};