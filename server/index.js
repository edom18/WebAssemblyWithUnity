var express = require('express');
var app = express();

app.get('/files/:fileName', function (req, res) {
    console.log(req.params);
    const file = `${__dirname}/files/${req.params.fileName}`;
    res.download(file);
});

app.listen(8080);