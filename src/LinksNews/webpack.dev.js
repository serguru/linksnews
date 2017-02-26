//const autoprefixer = require('autoprefixer');

var path = require('path');

var webpack = require('webpack');

var HtmlWebpackPlugin = require('html-webpack-plugin');
var CopyWebpackPlugin = require('copy-webpack-plugin');
var CleanWebpackPlugin = require('clean-webpack-plugin');

console.log("@@@@@@@@@ USING DEVELOPMENT @@@@@@@@@@@@@@@");

module.exports = {

  //  postcss: [autoprefixer],

    devtool: 'source-map',

    entry: {
        'app': './angular2App/main.ts' // JiT compilation
    },

    output: {
        path: "./wwwroot/",
        filename: 'dist/[name].bundle.js',
        publicPath: "/"
    },

    resolve: {
        extensions: ['.ts', '.js', '.json', '.css', '.scss', '.html']
    },

    devServer: {
        historyApiFallback: true,
        stats: 'minimal',
        outputPath: path.join(__dirname, 'wwwroot/')
    },

    module: {
        rules: [
            {
                test: /\.ts$/,
                loaders: [
                    'awesome-typescript-loader',
                    'angular2-template-loader',
                    'source-map-loader'
                ]
            },
            {
                test: /\.(png|jpg|gif|ico|woff|woff2|ttf|svg|eot)$/,
                exclude: /node_modules/,
                loader: "file-loader?name=assets/[name]-[hash:6].[ext]",
            },
            {
                test: /\.css$/,
                exclude: /node_modules/,
                loader: "style-loader!css-loader"
            },
            {
                test: /\.scss$/,
                exclude: /node_modules/,
                loaders: ["style-loader", "css-loader", "sass-loader"]
            },
            {
                test: /\.html$/,
                loader: 'raw-loader'
            },

            //{ test: /\.scss$/, loaders: ['style', 'css', 'postcss', 'sass'] },
            //{ test: /\.(woff2?|ttf|eot|svg)$/, loader: 'url?limit=10000' },
            // Bootstrap 4
            //{ 
            //    test: /bootstrap\/dist\/js\/umd\//, 
            //    loader: 'imports?jQuery=jquery' 
            //}
        ],
        exprContextCritical: false
    },

    plugins: [
        new CleanWebpackPlugin(
            [
                './wwwroot/dist',
                './wwwroot/i18n',
                './wwwroot/css',
                './wwwroot/fonts',
                './wwwroot/assets'
            ]
        ),

        new HtmlWebpackPlugin({
            filename: 'index.html',
            inject: 'body',
            template: 'angular2App/index.html'
        }),

        new CopyWebpackPlugin([
            { from: './angular2App/images/*.*', to: "assets/", flatten: true },

            { from: './angular2App/i18n/*.*', to: "i18n/", flatten: true },

            { from: './angular2App/css/bootstrap-flex.min.css', to: "css/", flatten: true },

            { from: './angular2App/css/font-awesome.min.css', to: "css/", flatten: true },

            { from: './angular2App/css/site.css', to: "css/", flatten: true },

            { from: './angular2App/fonts/*.*', to: "fonts/", flatten: true }
        ]),

        new webpack.ProvidePlugin({
            jQuery: 'jquery',
            $: 'jquery',
            jquery: 'jquery',
            "Tether": 'tether',
            "window.Tether": "tether"
        })    

    ]

};

