async function f() {
    let mysql = require("mysql");

    let connection = mysql.createConnection({
        host: "localhost",
        database: "async_await",
        user: "root",
        password: ""
    });

    try {
        await new Promise((resolve, reject) => {
            connection.connect((err) => {
                if (err) {
                    reject(err);
                } else {
                    console.log("Conectado");
                    resolve();
                }
            });
        });

        console.log("1");

        const categorias = "SELECT * FROM `material`;";
        const lista = await new Promise((resolve, reject) => {
            connection.query(categorias, (error, results) => {
                if (error) {
                    reject(error);
                } else {
                    resolve(results);
                }
            });
        });

        console.log(lista);

        console.log("2");

        connection.end();
    } catch (error) {
        console.error("Error:", error);
        connection.end();
    }
}

f();
