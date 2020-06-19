import {terser} from 'rollup-plugin-terser';

export default {
    input: { 
        fields: './scripts/src/fields.js',
        elements: './scripts/src/elements.js'
    },
    output: [{
        dir: './scripts/dist',
        entryFileNames: '[name].js',
        format: 'commonjs',
        esModule: false,
        strict: false
    },
    {
        dir: './scripts/dist',
        entryFileNames: '[name].min.js',
        format: 'commonjs',
        esModule: false,
        strict: false,
        plugins: [terser()]
    }]
};