/** @type {import('next').NextConfig} */
const nextConfig = {
    images: {
        domains: [
            'cdn.pixabay.com'
        ]
    },
    //TODO: remove the lines below and fix the issue at build (npm run build)
    typescript: {
        ignoreBuildErrors: true,
    },
    output: 'standalone'
};

export default nextConfig;
