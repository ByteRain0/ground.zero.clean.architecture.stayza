export function getRandom10DigitNumber() {
  // Generates a number between 1_000_000_000 and 9_999_999_999
  return Math.floor(1_000_000_000 + Math.random() * 9_000_000_000);
}

export function getRandomName() {
  const firstNames = ['Alex', 'Mia', 'Liam', 'Sofia', 'Noah', 'Ava', 'Ethan', 'Isla', 'Lucas', 'Zoe'];
  const lastNames = ['Johnson', 'Smith', 'Brown', 'Taylor', 'Anderson', 'Lee', 'Walker', 'Hall', 'Young', 'King'];

  const first = firstNames[Math.floor(Math.random() * firstNames.length)];
  const last = lastNames[Math.floor(Math.random() * lastNames.length)];

  return `${first} ${last}`;
}