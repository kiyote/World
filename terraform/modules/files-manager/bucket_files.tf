locals {
  files_bucket = {
    arn  = aws_s3_bucket.files_bucket.arn
    name = aws_s3_bucket.files_bucket.id
  }
}

resource "aws_s3_bucket" "files_bucket" {
  bucket = "${var.object_prefix}files"

  # Causes any Terraform that would destroy the bucket to be rejected
  lifecycle {
    prevent_destroy = true
  }
}

resource "aws_s3_bucket_versioning" "files_bucket_versioning" {
  bucket = aws_s3_bucket.files_bucket.id
  versioning_configuration {
    status = "Enabled"
  }
}

resource "aws_s3_bucket_server_side_encryption_configuration" "files_bucket_encryption" {
  bucket = aws_s3_bucket.files_bucket.id
  # Enable AES256 as the default server-side encryption for items
  rule {
    apply_server_side_encryption_by_default {
      sse_algorithm = "AES256"
    }
    bucket_key_enabled = true
  }
}

resource "aws_s3_bucket_acl" "files_bucket_acl" {
  bucket = aws_s3_bucket.files_bucket.id
  acl    = "private"
}

resource "aws_s3_bucket_lifecycle_configuration" "files_bucket_lifecycles" {
  bucket = aws_s3_bucket.files_bucket.id

  # Lifecycle to remove old versions of files after cleanup_delay_in_days
  rule {
    id     = "cleanup"
    status = "Enabled"

    filter {
    }

    noncurrent_version_expiration {
      noncurrent_days = 1
    }

    # Expired object delete markers are automatically deleted
    expiration {
      expired_object_delete_marker = true
    }

    abort_incomplete_multipart_upload {
      days_after_initiation = 1
    }
  }  
}

# The policy document for the files bucket
data "aws_iam_policy_document" "files_bucket_policy_document" {

  # Nobody is allowed to issue DeleteBucket on this bucket
  #statement {
  #  effect = "Deny"
  #  actions = [
  #    "s3:DeleteBucket",
  #  ]
  #  resources = [aws_s3_bucket.files_bucket.arn]
  #  principals {
  #    type        = "*"
  #    identifiers = ["*"]
  #  }
  #}

  # This will force any action to be denied if it's not coming from a SecureTransport connection
  statement {
    effect = "Deny"
    actions = [
      "s3:*",
    ]
    resources = [
      "${aws_s3_bucket.files_bucket.arn}/*",
    ]
    condition {
      test     = "Bool"
      variable = "aws:SecureTransport"
      values = [
        "false",
      ]
    }
    principals {
      type        = "*"
      identifiers = ["*"]
    }
  }
}

# Associates the above policy with the storage bucket
resource "aws_s3_bucket_policy" "files_bucket_policy" {
  bucket = aws_s3_bucket.files_bucket.id
  policy = data.aws_iam_policy_document.files_bucket_policy_document.json
}

# Deny all public access to the bucket
resource "aws_s3_bucket_public_access_block" "files_bucket_access_block" {
  bucket                  = aws_s3_bucket.files_bucket.bucket
  block_public_acls       = true
  block_public_policy     = true
  ignore_public_acls      = true
  restrict_public_buckets = true
}
