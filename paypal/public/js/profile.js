import { initializeApp } from "https://www.gstatic.com/firebasejs/9.0.0/firebase-app.js";
import { getAuth, onAuthStateChanged } from "https://www.gstatic.com/firebasejs/9.0.0/firebase-auth.js";
import { getDatabase, ref, onValue } from "https://www.gstatic.com/firebasejs/9.0.0/firebase-database.js";

const firebaseConfig = {
    apiKey: "AIzaSyDAq5iYAalbx8mrj7PAUvhlDGeJiEEreP0",
    authDomain: "holocure-27638.firebaseapp.com",
    databaseURL: "https://holocure-27638-default-rtdb.asia-southeast1.firebasedatabase.app",
    projectId: "holocure-27638",
    storageBucket: "holocure-27638.appspot.com",
    messagingSenderId: "397627522507",
    appId: "1:397627522507:web:92cc3302a67f346eba0a4e",
    measurementId: "G-6T306ZY51M"
};

try {
    const app = initializeApp(firebaseConfig);
    console.log("Firebase initialized successfully");

    const auth = getAuth(app);
    const db = getDatabase(app);

    onAuthStateChanged(auth, (user) => {
        console.log("Auth state changed:", user ? "User logged in" : "No user");
        
        if (user) {
            console.log("User info:", {
                email: user.email,
                uid: user.uid
            });

            document.getElementById('userEmail').value = user.email;

            console.log("Attempting to get diamonds for user:", user.uid);
            const diamondsRef = ref(db, `users/${user.uid}/Diamonds`);
            
            onValue(diamondsRef, (snapshot) => {
                console.log("Diamond data:", snapshot.val());
                const diamonds = snapshot.val() || 0;
                document.getElementById('diamondCount').textContent = diamonds;
            }, (error) => {
                console.error("Error getting diamonds:", error);
            });

            const transactionsRef = ref(db, `users/${user.uid}/transactions`);
            onValue(transactionsRef, (snapshot) => {
                console.log("Transactions data:", snapshot.val());
                const transactions = snapshot.val() || {};
                
                const transactionsList = Object.entries(transactions)
                    .map(([key, transaction]) => ({
                        id: key,
                        ...transaction
                    }))
                    .sort((a, b) => b.timestamp - a.timestamp);

                const transactionsContainer = document.getElementById('transactionsList');
                transactionsContainer.innerHTML = transactionsList.length > 0 
                    ? transactionsList.map(transaction => `
                        <div class="transaction-item">
                            <div class="transaction-info">
                                <span class="transaction-product">${transaction.productTitle}</span>
                                <span class="transaction-date">${new Date(transaction.timestamp).toLocaleDateString()}</span>
                            </div>
                            <div class="transaction-details">
                                <span class="transaction-amount">$${transaction.amount}</span>
                                <span class="transaction-diamonds">+${transaction.diamonds} 💎</span>
                            </div>
                        </div>
                    `).join('')
                    : '<div class="no-transactions">No transactions yet</div>';
            }, (error) => {
                console.error("Error getting transactions:", error);
            });
        } else {
            console.log("Redirecting to login page...");
            window.location.href = '/login';
        }
    }, (error) => {
        console.error("Auth state change error:", error);
    });

} catch (error) {
    console.error("Firebase initialization error:", error);
}

window.logout = function() {
    const auth = getAuth();
    auth.signOut()
        .then(() => {
            console.log("Logged out successfully");
            window.location.href = '/login';
        })
        .catch((error) => {
            console.error("Logout error:", error);
            console.error("Logout error details:", {
                code: error.code,
                message: error.message,
                stack: error.stack
            });
        });
}

window.editProfile = function() {
    alert('Edit profile functionality will be implemented soon!');
} 