<!-- ImageUpload.vue -->

<template>
    <div class="image-upload-container">
        <div class="label-container">
            <label>{{ label }}</label>
        </div>
        <div class="file-input-container">
            <input type="file" @change="handleFileChange" class="file-input" />
        </div>
        <div class="image-container" @mouseover="zoomIn" @mouseleave="zoomOut">
            <label class="image-label" v-if="labelInside">{{ label }}</label>
            <img v-if="localImageFile"
                 :src="localImageFile"
                 alt="Uploaded Image"
                 class="preview-image" />
        </div>
    </div>
</template>

<script>
    export default {
        props: {
            label: {
                type: String,
                required: true,
            },
            imageFile: {
                type: String,
                required: true,
            },
            labelInside: {
                type: Boolean,
                default: false,
            },
        },
        data() {
            return {
                localImageFile: this.imageFile,
                isZoomed: false,
            };
        },
        methods: {
            handleFileChange(event) {
                // Perform any file handling logic here
                const file = event.target.files[0];
                // Update the local data property
                this.localImageFile = URL.createObjectURL(file);
                // Emit an event to notify the parent component about the change
                this.$emit('update:imageFile', this.localImageFile);
            },
            zoomIn() {
                this.isZoomed = true;
            },
            zoomOut() {
                this.isZoomed = false;
            },
        },
    };
</script>

<style scoped>

    .image-upload-container {
        position: relative;
    }

    .file-input-container {
        position: relative;
        display: inline-block;
    }

    .file-input {
        width: 100%;
        box-sizing: border-box;
        display: block;
        border: 1px solid currentColor;
        padding: calc(var(--size-bezel) * 1.5) var(--size-bezel);
        color: currentColor;
        background: transparent;
        border-radius: var(--size-radius);
        /*border: 1px solid #ccc;*/ /* Adjust the border style and color as needed */
        /*padding: 8px;*/ /* Adjust the padding as needed */
        /*margin-top: 8px;*/ /* Adjust the margin as needed */
    }

    .image-container {
        position: relative;
        overflow: hidden;
    }

    .preview-image {
        max-width: 50%; /* Adjust the maximum width as needed */
        max-height: 50%; /* Adjust the maximum height as needed */
        transition: transform 0.3s ease-in-out;
    }

    .image-container:hover .preview-image {
        transform: scale(1.2); /* Adjust the zoom level as needed */
    }

    .image-label {
        margin-left: 17px;
        cursor: text;
        font-size: 14px;
        color: var(--color-accent);
        font-weight: bold;
        /*position: absolute;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        color: #333;*/ /* Adjust the label color as needed */
        /*font-size: 14px;*/ /* Adjust the font size as needed */
    }
</style>
