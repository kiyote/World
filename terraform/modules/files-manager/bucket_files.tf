
resource "aws_s3_bucket" "files_bucket" {
  bucket = "${var.object_prefix}files"
  acl    = "private"

  versioning {
    enabled = false
  }

  # Lifecycle to remove old versions of files after cleanup_delay_in_days
  lifecycle_rule {
    id      = "cleanup"
    enabled = true

    noncurrent_version_expiration {
      days = 1
    }

    # Expired object delete markers are automatically deleted
    expiration {
      expired_object_delete_marker = true
    }

    abort_incomplete_multipart_upload_days = 1
  }

  # Causes any Terraform that would destroy the bucket to be rejected
  lifecycle {
    prevent_destroy = true
  }
}

locals {
  files_bucket = {
    arn  = aws_s3_bucket.files_bucket.arn
    name = aws_s3_bucket.files_bucket.id
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
