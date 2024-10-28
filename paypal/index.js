require('dotenv').config();
const express = require('express');
const paypal = require('./services/paypal');
const path = require('path');
const admin = require('firebase-admin');
const cookieParser = require('cookie-parser');
const serviceAccount = require('d:/UnityProject/serviceAccountKey.json'); // Thay đổi đường dẫn
const session = require('express-session');
const helmet = require('helmet');
const app = express();
// Sử dụng cookie-parser middleware
app.use(cookieParser());

// Add session middleware
app.use(session({
    secret: 'a8!Z@kLFJj8^sdHj39&klmdJf8jlWle9sjdI',
    resave: false,
    saveUninitialized: true,
    cookie: { secure: process.env.NODE_ENV === 'production' }
  }));
  

// Initialize Firebase Admin SDK
admin.initializeApp({
  credential: admin.credential.cert(serviceAccount),
  databaseURL: "https://holocure-27638-default-rtdb.asia-southeast1.firebasedatabase.app"
});

// Middleware
app.use(express.static(path.join(__dirname, 'public')));
app.use(express.urlencoded({ extended: true })); // Để xử lý form POST
app.set('view engine', 'ejs');
app.use(express.json());

// Mock data for products (you can replace this with actual database queries later)
const products = [
    { id: '1', title: 'Node.js Complete Course', price: 100.00, description: 'Node.js Complete Course with Express and MongoDB', diamonds: 1000 },
    { id: '2', title: 'React Masterclass', price: 89.99, description: 'Comprehensive React course including Redux and React Hooks', diamonds: 900 },
    { id: '3', title: 'Python for Data Science', price: 79.99, description: 'Learn Python for data analysis and machine learning', diamonds: 800 },
    { id: '4', title: 'Full Stack Web Development', price: 149.99, description: 'Become a full stack developer with this comprehensive course', diamonds: 1500 },
    { id: '5', title: 'Mobile App Development with Flutter', price: 94.99, description: 'Build cross-platform mobile apps with Flutter and Dart', diamonds: 950 },
    { id: '6', title: 'DevOps Essentials', price: 129.99, description: 'Master DevOps practices and tools for modern software development', diamonds: 1300 }
];
// Routes
app.get('/', (req, res) => {
    const sessionCookie = req.cookies.session || '';

    admin
        .auth()
        .verifySessionCookie(sessionCookie, true)
        .then((decodedClaims) => {
            res.render('index', { user: decodedClaims, products: products });
        })
        .catch((error) => {
            res.render('index', { user: null, products: products });
        });
});
app.get('/products', (req, res) => {
    // Fetch products from your database or API
    const products = [
        { id: '1', title: 'Node.js Complete Course', price: 100.00, description: 'Node.js Complete Course with Express and MongoDB', diamonds: 1000 },
        { id: '2', title: 'React Masterclass', price: 89.99, description: 'Comprehensive React course including Redux and React Hooks', diamonds: 900 },
        { id: '3', title: 'Python for Data Science', price: 79.99, description: 'Learn Python for data analysis and machine learning', diamonds: 800 },
        { id: '4', title: 'Full Stack Web Development', price: 149.99, description: 'Become a full stack developer with this comprehensive course', diamonds: 1500 },
        { id: '5', title: 'Mobile App Development with Flutter', price: 94.99, description: 'Build cross-platform mobile apps with Flutter and Dart', diamonds: 950 },
        { id: '6', title: 'DevOps Essentials', price: 129.99, description: 'Master DevOps practices and tools for modern software development', diamonds: 1300 },
        { id: '6', title: 'DevOps Essentials', price: 129.99, description: 'Master DevOps practices and tools for modern software development', diamonds: 1300 },
        { id: '6', title: 'DevOps Essentials', price: 129.99, description: 'Master DevOps practices and tools for modern software development', diamonds: 1300 },
    ];
    res.render('products', { products: products });
});
app.get('/character', (req, res) => {
    res.render('character');
});
app.get('/characters/amelia', (req, res) => {
    res.render('characters/amelia');
});
app.get('/characters/gura', (req, res) => {
    res.render('characters/gura');
});
app.get('/characters/calli', (req, res) => {
    res.render('characters/calli');
});
app.get('/characters/kiara', (req, res) => {
    res.render('characters/kiara');
});
app.get('/characters/ina', (req, res) => {
    res.render('characters/ina');
});
app.get('/weapon', (req, res) => {
    res.render('weapon');
});
app.get('/weapons/pistol-shot', (req, res) => {
    res.render('weapons/pistol-shot');
});
app.get('/weapons/phoenix-sword', (req, res) => {
    res.render('weapons/phoenix-sword');
});
app.get('/weapons/trident-thrust', (req, res) => {
    res.render('weapons/trident-thrust');
});
app.get('/weapons/summon-tentacle', (req, res) => {
    res.render('weapons/summon-tentacle');
});
app.get('/weapons/scythe-swing', (req, res) => {
    res.render('weapons/scythe-swing');
});
// Route kiểm tra đăng nhập
app.get('/check-auth', (req, res) => {
    const sessionCookie = req.cookies.session || '';

    admin.auth().verifySessionCookie(sessionCookie, true)
        .then(() => {
            res.json({ isAuthenticated: true });
        })
        .catch(() => {
            res.json({ isAuthenticated: false });
        });
});

app.get('/login', (req, res) => {
    res.render('login', { redirect: req.query.redirect || '/' });
});

app.post('/login', async (req, res) => {
    const { email, password } = req.body;

    try {
        // Xác thực người dùng với Firebase Admin SDK
        const userRecord = await admin.auth().getUserByEmail(email);
        
        // Nếu xác thực thành công, tạo session hoặc token
        // (Đây chỉ là ví dụ, bạn nên sử dụng phương pháp bảo mật hơn trong thực tế)
        req.session.userId = userRecord.uid;
        
        res.json({ success: true, message: 'Đăng nhập thành công' });
    } catch (error) {
        console.error('Lỗi đăng nhập:', error);
        res.status(400).json({ success: false, message: 'Đăng nhập thất bại' });
    }
});

app.get('/register', (req, res) => {
    res.render('register');
});

app.post('/register', (req, res) => {
    // Xử lý đăng ký ở đây
    res.send('Xử lý đăng ký');
});

// Route xử lý thanh toán
app.get('/pay/:productId', (req, res) => {
    const sessionCookie = req.cookies.session || '';

    admin.auth().verifySessionCookie(sessionCookie, true)
        .then(() => {
            // Người dùng đã đăng nhập, xử lý thanh toán ở đây
            res.send(`Processing payment for product ${req.params.productId}`);
        })
        .catch(() => {
            // Người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập
            res.redirect(`/login?redirect=${encodeURIComponent(`/pay/${req.params.productId}`)}`);
        });
});

app.post('/pay', async (req, res) => {
    try {
        const url = await paypal.createOrder();
        res.redirect(url);
    } catch (error) {
        res.send('Error: ' + error);
    }
});

app.get('/complete-order', async (req, res) => {
    try {
        const result = await paypal.capturePayment(req.query.token);
        const orderId = result.id;
        const productId = req.session.productId;
        const product = products.find(p => p.id === productId);

        if (!product) {
            throw new Error('Product not found');
        }

        const sessionCookie = req.cookies.session || '';
        let user = null;

        try {
            const decodedClaims = await admin.auth().verifySessionCookie(sessionCookie, true);
            user = {
                email: decodedClaims.email,
                uid: decodedClaims.uid
            };
            await updateUserDiamonds(user.uid, product.diamonds);
        } catch (error) {
            console.error('Error verifying session:', error);
            // Proceed without user information if session verification fails
        }

        res.render('success', { 
            user: user,
            result: result,
            productInfo: product
        });
    } catch (error) {
        console.error('Error completing order:', error);
        res.render('error', { message: 'Error completing order', error });
    }
});

app.get('/cancel-order', (req, res) => {
    res.render('cancel');
});

app.post('/sessionLogin', async (req, res) => {
    if (!req.body || !req.body.idToken) {
        return res.status(400).json({ error: 'Missing idToken' });
    }

    const idToken = req.body.idToken.toString();
    const expiresIn = 60 * 60 * 24 * 5 * 1000; // 5 days

    try {
        const sessionCookie = await admin.auth().createSessionCookie(idToken, { expiresIn });
        const options = { maxAge: expiresIn, httpOnly: true, secure: true };
        res.cookie('session', sessionCookie, options);
        res.json({ status: 'success' });
    } catch (error) {
        console.error('Session creation error:', error);
        res.status(401).json({ error: 'UNAUTHORIZED REQUEST!' });
    }
});


app.use((err, req, res, next) => {
  // set locals, only providing error in development
  res.locals.message = err.message;
  res.locals.error = req.app.get('env') === 'development' ? err : {};

  // render the error page
  res.status(err.status || 500);
  res.render('error', {
    message: err.message,
    error: err
  });
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => console.log(`Server running on port ${PORT}`));

app.post('/create-paypal-order', async (req, res) => {
    try {
        const { productId } = req.body;
        const product = products.find(p => p.id === productId);

        if (!product) {
            throw new Error('Product not found');
        }

        req.session.productId = productId; // Store productId in session
        const url = await paypal.createOrder(product);
        res.json({ approvalUrl: url });
    } catch (error) {
        console.error('Error creating PayPal order:', error);
        res.status(500).json({ error: 'Error creating PayPal order' });
    }
});

// Add this function to update diamonds in Firebase
async function updateUserDiamonds(userId, diamondsToAdd) {
    const db = admin.database();
    const userRef = db.ref(`users/${userId}/Diamonds`);
    
    try {
        await userRef.transaction((currentDiamonds) => {
            return (currentDiamonds || 0) + diamondsToAdd;
        });
        console.log(`Updated diamonds for user ${userId}`);
    } catch (error) {
        console.error('Error updating diamonds:', error);
        throw error;
    }
}

// Update or add this configuration
app.use(
  helmet.contentSecurityPolicy({
    directives: {
      defaultSrc: ["'self'"],
      imgSrc: [
        "'self'",
        'https://www.paypal.com',
        'https://www.paypalobjects.com',
        'https://t.paypal.com',
        'https://stats.g.doubleclick.net',
        'https://www.google-analytics.com',
        'https://www.googletagmanager.com',
        'https://www.google.com',
        'https://www.gstatic.com',
        'data:',
        // Add the URL that was causing the error here
        'https://example.com', // Replace with the actual URL from your error message
      ],
      // Add other directives as needed
    },
  })
);
